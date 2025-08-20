using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BizOpsAPI.DTOs;
using BizOpsAPI.Helpers;
using BizOpsAPI.Repositories;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Tnef;

namespace BizOpsAPI.Services
{
    public class EmailReceiptIngestionService : BackgroundService
    {
        private readonly EmailIngestionSettings _cfg;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailReceiptIngestionService> _log;

        public EmailReceiptIngestionService(
            IOptions<EmailIngestionSettings> cfg,
            IServiceScopeFactory scopeFactory,
            ILogger<EmailReceiptIngestionService> log)
        {
            _cfg = cfg.Value;
            _scopeFactory = scopeFactory;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stop)
        {
            if (!_cfg.Enable)
            {
                _log.LogInformation("[Ingestion] Disabled via config.");
                return;
            }

            // Compile subject regex safely
            Regex subjectRegex;
            try
            {
                subjectRegex = new Regex(
                    string.IsNullOrWhiteSpace(_cfg.SubjectRegex) ? "#(?<id>\\d+)" : _cfg.SubjectRegex,
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "[Ingestion] Invalid SubjectRegex in config. Falling back to default '#(?<id>\\d+)'");
                subjectRegex = new Regex("#(?<id>\\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            // Exponential backoff state for auth failures
            var consecutiveAuthFailures = 0;

            while (!stop.IsCancellationRequested)
            {
                try
                {
                    var password = ResolvePassword();
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        _log.LogError("[Ingestion] No App Password configured. Set EmailIngestion:AppPassword or env var EMAIL_INGESTION_PASSWORD. Pausing 60s.");
                        await Task.Delay(TimeSpan.FromSeconds(60), stop);
                        continue;
                    }

                    using var imap = CreateImapClientWithOptionalProtocolLog();
                    imap.AuthenticationMechanisms.Remove("XOAUTH2"); // force simple auth with App Password

                    _log.LogDebug("[Ingestion] Connecting to IMAP {Server}:{Port} ...", _cfg.ImapServer, _cfg.Port);
                    await imap.ConnectAsync(_cfg.ImapServer, _cfg.Port, SecureSocketOptions.SslOnConnect, stop);

                    try
                    {
                        await imap.AuthenticateAsync(_cfg.Username, password, stop);
                    }
                    catch (AuthenticationException authEx)
                    {
                        consecutiveAuthFailures++;
                        var delay = ComputeBackoff(_cfg.PollSeconds, consecutiveAuthFailures);
                        _log.LogError(authEx, "[Ingestion] Authentication failed for {User}. Using App Password with IMAP. Backing off {Delay}s (attempt {Count}).",
                            _cfg.Username, (int)delay.TotalSeconds, consecutiveAuthFailures);

                        try { await imap.DisconnectAsync(true, stop); } catch { /* ignore */ }

                        await Task.Delay(delay, stop);
                        continue;
                    }

                    // Reset backoff after successful auth
                    consecutiveAuthFailures = 0;

                    var inbox = (IImapFolder)await imap.GetFolderAsync(_cfg.Folder, stop);
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stop);

                    _log.LogInformation("[Ingestion] Connected {Server}, folder={Folder}", _cfg.ImapServer, _cfg.Folder);

                    // Build search query: delivered after 'since' and optionally only NotSeen
                    var since = DateTime.UtcNow.AddDays(-(_cfg.LookbackDays > 0 ? _cfg.LookbackDays : 2));
                    SearchQuery query = SearchQuery.DeliveredAfter(since);
                    if (!_cfg.ProcessSeen) query = query.And(SearchQuery.NotSeen);

                    var ids = await inbox.SearchAsync(query, stop);
                    _log.LogInformation("[Ingestion] {Count} candidate message(s) since {Since:u} (ProcessSeen={Seen})",
                        ids.Count, since, _cfg.ProcessSeen);

                    if (ids.Count == 0)
                    {
                        await imap.DisconnectAsync(true, stop);
                        await Task.Delay(TimeSpan.FromSeconds(Math.Max(5, _cfg.PollSeconds)), stop);
                        continue;
                    }

                    // Prefetch summaries (UID, envelope, gmail thread id)
                    var summaries = await inbox.FetchAsync(
                        ids,
                        MessageSummaryItems.UniqueId |
                        MessageSummaryItems.Envelope |
                        MessageSummaryItems.GMailThreadId);

                    using var scope = _scopeFactory.CreateScope();
                    var receipts    = scope.ServiceProvider.GetRequiredService<IReceiptService>();
                    var invoiceRepo = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

                    foreach (var s in summaries)
                    {
                        try
                        {
                            int? invoiceId = null;
                            MimeMessage? fullMsg = null;

                            // 1) Try matching by Gmail thread id (if present)
                            var threadId = s.GMailThreadId; // ulong?
                            if (threadId.HasValue && threadId.Value != 0UL)
                            {
                                var threadIdStr = threadId.Value.ToString("D", CultureInfo.InvariantCulture);
                                var invByThread = await invoiceRepo.GetByEmailThreadIdAsync(threadIdStr);
                                if (invByThread != null)
                                {
                                    invoiceId = invByThread.InvoiceId;
                                    _log.LogDebug("[Ingestion] UID {Uid} matched by GmailThreadId {ThreadId} → Invoice #{InvoiceId}",
                                        s.UniqueId.Id, threadIdStr, invoiceId);
                                }
                            }

                            // 2) Match by In-Reply-To / References against original invoice Message-Id
                            if (invoiceId == null)
                            {
                                try
                                {
                                    fullMsg ??= await inbox.GetMessageAsync(s.UniqueId, stop);

                                    static string Norm(string v) => v?.Trim().Trim('<', '>') ?? string.Empty;

                                    var refsHeader = fullMsg.Headers["References"] ?? string.Empty;
                                    var inReplyTo  = fullMsg.Headers["In-Reply-To"] ?? string.Empty;

                                    var refIds = refsHeader
                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(Norm)
                                        .Where(x => !string.IsNullOrEmpty(x))
                                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                                    var irt = Norm(inReplyTo);
                                    if (!string.IsNullOrEmpty(irt))
                                        refIds.Add(irt);

                                    _log.LogDebug("[Ingestion] UID {Uid} reference ids count={Count}", s.UniqueId.Id, refIds.Count);

                                    if (refIds.Count > 0)
                                    {
                                        // Consider replacing with a targeted method GetByAnyMessageIdAsync(refIds)
                                        var all = await invoiceRepo.GetAllAsync();
                                        var matched = all.FirstOrDefault(i =>
                                            !string.IsNullOrWhiteSpace(i.EmailMessageId) &&
                                            refIds.Contains(i.EmailMessageId.Trim('<', '>')));

                                        if (matched != null)
                                        {
                                            invoiceId = matched.InvoiceId;
                                            _log.LogDebug("[Ingestion] UID {Uid} matched by In-Reply-To/References → Invoice #{InvoiceId}",
                                                s.UniqueId.Id, invoiceId);
                                        }
                                    }
                                }
                                catch (Exception exHdr)
                                {
                                    _log.LogDebug(exHdr, "[Ingestion] Unable to read headers for UID {Uid}", s.UniqueId.Id);
                                }
                            }

                            // 3) Fallback: subject token like "#123"
                            if (invoiceId == null)
                            {
                                var subj = s.Envelope?.Subject ?? string.Empty;
                                var m = subjectRegex.Match(subj);
                                if (m.Success && int.TryParse(m.Groups["id"].Value, out var parsed))
                                {
                                    invoiceId = parsed;
                                    _log.LogDebug("[Ingestion] UID {Uid} matched by Subject token → Invoice #{InvoiceId}",
                                        s.UniqueId.Id, invoiceId);
                                }
                            }

                            if (invoiceId == null)
                            {
                                _log.LogDebug("[Ingestion] UID {Uid} skipped: no thread/headers/subject match.", s.UniqueId.Id);
                                continue;
                            }

                            // Optional To-filter:
                            if (!string.IsNullOrWhiteSpace(_cfg.ToFilter))
                            {
                                bool allowed;
                                if (_cfg.ToFilter.Contains("@"))
                                {
                                    allowed =
                                        (s.Envelope?.To?.Any(a => a.ToString().Contains(_cfg.ToFilter, StringComparison.OrdinalIgnoreCase)) ?? false) ||
                                        (s.Envelope?.Cc?.Any(a => a.ToString().Contains(_cfg.ToFilter, StringComparison.OrdinalIgnoreCase)) ?? false) ||
                                        (s.Envelope?.Bcc?.Any(a => a.ToString().Contains(_cfg.ToFilter, StringComparison.OrdinalIgnoreCase)) ?? false);
                                }
                                else
                                {
                                    var subj = s.Envelope?.Subject ?? string.Empty;
                                    var from = s.Envelope?.From?.ToString() ?? string.Empty;
                                    allowed = subj.Contains(_cfg.ToFilter, StringComparison.OrdinalIgnoreCase) ||
                                              from.Contains(_cfg.ToFilter, StringComparison.OrdinalIgnoreCase);
                                }

                                if (!allowed)
                                {
                                    _log.LogDebug("[Ingestion] UID {Uid} skipped by ToFilter '{Filter}'.", s.UniqueId.Id, _cfg.ToFilter);
                                    continue;
                                }
                            }

                            // Fetch full message (reuse if already loaded)
                            fullMsg ??= await inbox.GetMessageAsync(s.UniqueId, stop);

                            _log.LogDebug("[Ingestion] UID {Uid} RootBodyType={Type} Subject='{Subject}' From='{From}' To='{To}'",
                                s.UniqueId.Id,
                                fullMsg.Body?.ContentType?.MimeType ?? "(null)",
                                fullMsg.Subject,
                                fullMsg.From,
                                fullMsg.To);

                            // === Collect ALL files from the message (any type) ===
                            var files = ExtractAllFiles(fullMsg, stop).ToList();

                            _log.LogDebug("[Ingestion] UID {Uid} total files discovered={Count}", s.UniqueId.Id, files.Count);

                            if (files.Count == 0)
                            {
                                _log.LogWarning("[Ingestion] UID {Uid} skipped: no files found in MIME tree.", s.UniqueId.Id);
                                continue;
                            }

                            var savedAny = false;

                            foreach (var file in files)
                            {
                                try
                                {
                                    var saved = await receipts.CreateFromEmailAsync(new ReceiptEmailCreateDto
                                    {
                                        InvoiceId  = invoiceId.Value,
                                        FileName   = file.FileName,
                                        Bytes      = file.Bytes,
                                        UploadedAt = fullMsg.Date.UtcDateTime
                                    }, stop);

                                    _log.LogInformation(
                                        "[Ingestion] ✅ Saved file {ReceiptId} for Invoice #{InvoiceId}: {File} ({Size} bytes)",
                                        saved.Id, invoiceId, file.FileName, file.Bytes.Length);

                                    savedAny = true;
                                }
                                catch (Exception ex)
                                {
                                    _log.LogError(ex,
                                        "[Ingestion] ❌ Failed to save file for Invoice #{InvoiceId}: {File} ({Size} bytes)",
                                        invoiceId, file.FileName, file.Bytes.Length);
                                }
                            }

                            if (savedAny)
                            {
                                await inbox.AddFlagsAsync(s.UniqueId, MessageFlags.Seen, true, stop);

                                if (!string.IsNullOrWhiteSpace(_cfg.ProcessedFolder))
                                {
                                    try
                                    {
                                        await MoveOrLabelProcessedAsync(imap, inbox, s.UniqueId, _cfg.ProcessedFolder!, stop);
                                        _log.LogDebug("[Ingestion] UID {Uid} moved/labeled to '{Folder}'", s.UniqueId.Id, _cfg.ProcessedFolder);
                                    }
                                    catch (Exception exMove)
                                    {
                                        _log.LogWarning(exMove, "[Ingestion] Move/label failed for UID {Uid}", s.UniqueId.Id);
                                    }
                                }
                            }
                            else
                            {
                                _log.LogDebug("[Ingestion] UID {Uid} left in place (no files saved).", s.UniqueId.Id);
                            }
                        }
                        catch (Exception exPerMsg)
                        {
                            _log.LogError(exPerMsg, "[Ingestion] Exception processing UID {Uid}", s.UniqueId.Id);
                        }
                    }

                    await imap.DisconnectAsync(true, stop);
                }
                catch (OperationCanceledException) when (stop.IsCancellationRequested)
                {
                    // shutting down
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "[Ingestion] Poll cycle failed (non-auth).");
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Max(5, _cfg.PollSeconds)), stop);
            }
        }

        private ImapClient CreateImapClientWithOptionalProtocolLog()
        {
            if (!string.IsNullOrWhiteSpace(_cfg.ProtocolLogDir))
            {
                // Move relative paths outside the project to avoid dotnet-watch reloads
                var expanded = Environment.ExpandEnvironmentVariables(_cfg.ProtocolLogDir);
                if (!Path.IsPathRooted(expanded))
                {
                    var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    expanded = Path.Combine(baseDir, "BizOpsAPI", expanded);
                }

                Directory.CreateDirectory(expanded);
                var path = Path.Combine(expanded, $"imap_{DateTime.UtcNow:yyyyMMdd}.log");
                var logger = new ProtocolLogger(path, append: true);
                return new ImapClient(logger);
            }
            return new ImapClient();
        }

        private async Task MoveOrLabelProcessedAsync(ImapClient imap, IImapFolder inbox, UniqueId id, string processedPath, CancellationToken ct)
        {
            try
            {
                var processed = await EnsureFolderPathAsync(imap, processedPath, ct);
                await inbox.MoveToAsync(id, processed, ct);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "[Ingestion] Move to processed folder failed; attempting to label instead.");

                if (imap.Capabilities.HasFlag(ImapCapabilities.GMailExt1))
                {
                    try
                    {
                        await inbox.AddLabelsAsync(id, new[] { processedPath }, true, ct);
                        _log.LogInformation("[Ingestion] Applied Gmail label '{Label}' to message {Uid}", processedPath, id.Id);
                    }
                    catch (Exception ex2)
                    {
                        _log.LogError(ex2, "[Ingestion] Failed to apply Gmail label '{Label}'", processedPath);
                    }
                }
                else
                {
                    _log.LogWarning("[Ingestion] Server does not support Gmail labels and move failed; message left in INBOX.");
                }
            }
        }

        private static async Task<IImapFolder> EnsureFolderPathAsync(ImapClient imap, string fullPath, CancellationToken ct)
        {
            var root = await imap.GetFolderAsync(imap.PersonalNamespaces[0].Path, ct);
            var rootImap = (IImapFolder)root;

            var parts = fullPath.Split(new[] { '/', '\\', rootImap.DirectorySeparator }, StringSplitOptions.RemoveEmptyEntries);

            IImapFolder current = rootImap;
            foreach (var name in parts)
            {
                IImapFolder? next;
                try
                {
                    var sub = await current.GetSubfolderAsync(name, ct);
                    next = sub as IImapFolder ?? (IImapFolder)await imap.GetFolderAsync(sub.FullName, ct);
                }
                catch (FolderNotFoundException)
                {
                    var created = await current.CreateAsync(name, true, ct);
                    next = created as IImapFolder ?? (IImapFolder)await imap.GetFolderAsync(created.FullName, ct);
                }
                current = next!;
            }

            return current;
        }

        // ===============================
        // MIME traversal (ALL file types)
        // ===============================

        private static IEnumerable<(string FileName, byte[] Bytes)> ExtractAllFiles(MimeMessage msg, CancellationToken ct)
        {
            if (msg.Body == null) yield break;

            foreach (var r in ExtractFromEntity(msg.Body, ct))
                yield return r;
        }

        private static IEnumerable<(string FileName, byte[] Bytes)> ExtractFromEntity(MimeEntity entity, CancellationToken ct)
        {
            // 1) Simple file parts
            if (entity is MimePart part)
            {
                // Skip email body parts that are plain text/html without a filename
                var isText = part.ContentType?.MediaType.Equals("text", StringComparison.OrdinalIgnoreCase) == true;
                var isHtmlOrPlain =
                    part.ContentType?.MediaSubtype.Equals("plain", StringComparison.OrdinalIgnoreCase) == true ||
                    part.ContentType?.MediaSubtype.Equals("html", StringComparison.OrdinalIgnoreCase) == true;

                var hasFileName = !string.IsNullOrWhiteSpace(part.FileName);

                if (isText && isHtmlOrPlain && !hasFileName)
                {
                    // Treat as the email body — not a downloadable file
                    yield break;
                }

                using var ms = new MemoryStream();
                part.Content.DecodeTo(ms, ct);
                var bytes = ms.ToArray();

                var name = hasFileName ? part.FileName : GenerateFileNameFromContentType(part);

                if (bytes.Length > 0)
                    yield return (name, bytes);

                yield break;
            }

            // 2) Outlook TNEF wrapper (winmail.dat): extract inner attachments
            if (entity is TnefPart tnef)
            {
                foreach (var extracted in tnef.ExtractAttachments())
                {
                    foreach (var r in ExtractFromEntity(extracted, ct))
                        yield return r;
                }
                yield break;
            }

            // 3) Nested email (.eml)
            if (entity is MessagePart msgPart && msgPart.Message != null)
            {
                foreach (var r in ExtractAllFiles(msgPart.Message, ct))
                    yield return r;
                yield break;
            }

            // 4) Multiparts → recurse children
            if (entity is Multipart mp)
            {
                foreach (var child in mp)
                {
                    foreach (var r in ExtractFromEntity(child, ct))
                        yield return r;
                }
            }
        }

        private static string GenerateFileNameFromContentType(MimePart part)
        {
            var baseName = "file";
            var ct = part.ContentType;

            if (ct != null)
            {
                var media = ct.MediaType?.ToLowerInvariant();
                var sub   = ct.MediaSubtype?.ToLowerInvariant();

                if (media == "application" && sub == "pdf") return $"{baseName}-{Guid.NewGuid():N}.pdf";
                if (media == "application" && sub == "zip") return $"{baseName}-{Guid.NewGuid():N}.zip";
                if (media == "image" && !string.IsNullOrWhiteSpace(sub)) return $"{baseName}-{Guid.NewGuid():N}.{sub}";
                if (media == "text" && sub == "csv") return $"{baseName}-{Guid.NewGuid():N}.csv";
                if (media == "text" && sub == "plain") return $"{baseName}-{Guid.NewGuid():N}.txt";
                if (media == "text" && sub == "html") return $"{baseName}-{Guid.NewGuid():N}.html";
                if (media == "application" && (sub?.Contains("msword") == true || sub == "vnd.openxmlformats-officedocument.wordprocessingml.document"))
                    return $"{baseName}-{Guid.NewGuid():N}.docx";
                if (media == "application" && (sub?.Contains("excel") == true || sub == "vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                    return $"{baseName}-{Guid.NewGuid():N}.xlsx";
                if (media == "application" && (sub?.Contains("powerpoint") == true || sub == "vnd.openxmlformats-officedocument.presentationml.presentation"))
                    return $"{baseName}-{Guid.NewGuid():N}.pptx";
            }

            return $"{baseName}-{Guid.NewGuid():N}.bin";
        }

        // ===============================
        // Helpers
        // ===============================

        private string? ResolvePassword()
        {
            // Primary: configured App Password. Fallback: env var.
            if (!string.IsNullOrWhiteSpace(_cfg.AppPassword))
                return _cfg.AppPassword;

            var env = Environment.GetEnvironmentVariable("EMAIL_INGESTION_PASSWORD");
            return string.IsNullOrWhiteSpace(env) ? null : env;
        }

        private static TimeSpan ComputeBackoff(int pollSeconds, int failures)
        {
            var baseSec = Math.Max(15, pollSeconds); // at least 15s
            var delay = TimeSpan.FromSeconds(Math.Min(300, baseSec * Math.Pow(2, Math.Clamp(failures - 1, 0, 10))));
            return delay;
        }
    }
}
