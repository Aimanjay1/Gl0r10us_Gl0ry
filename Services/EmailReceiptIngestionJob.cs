// Services/EmailReceiptIngestionJob.cs
using System.Globalization;
using System.Text.RegularExpressions;
using BizOpsAPI.DTOs;
using BizOpsAPI.Helpers;
using BizOpsAPI.Repositories;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Tnef;

namespace BizOpsAPI.Services
{
    public class EmailReceiptIngestionJob : IEmailReceiptIngestionJob
    {
        private readonly EmailIngestionSettings _cfg;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailReceiptIngestionJob> _log;

        // prevent overlapping runs from multiple clicks
        private static readonly SemaphoreSlim _gate = new(1, 1);

        public EmailReceiptIngestionJob(
            IOptions<EmailIngestionSettings> cfg,
            IServiceScopeFactory scopeFactory,
            ILogger<EmailReceiptIngestionJob> log)
        {
            _cfg = cfg.Value;
            _scopeFactory = scopeFactory;
            _log = log;
        }

        public async Task<EmailIngestionResult> RunOnceAsync(CancellationToken ct = default)
        {
            await _gate.WaitAsync(ct);
            try
            {
                if (!_cfg.Enable)
                {
                    _log.LogInformation("[Ingestion] Disabled via config; skipping.");
                    return new EmailIngestionResult(0,0,0,0);
                }

                // Compile subject regex safely
                Regex subjectRegex;
                try
                {
                    subjectRegex = new Regex(
                        string.IsNullOrWhiteSpace(_cfg.SubjectRegex) ? "#(?<id>\\d+)" : _cfg.SubjectRegex,
                        RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                catch
                {
                    subjectRegex = new Regex("#(?<id>\\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                var password = ResolvePassword();
                if (string.IsNullOrWhiteSpace(password))
                {
                    _log.LogError("[Ingestion] No App Password configured.");
                    return new EmailIngestionResult(0,0,0,0);
                }

                int messagesExamined = 0;
                int messagesMatched  = 0;
                int filesSaved       = 0;
                int receiptsCreated  = 0;

                using var imap = CreateImapClientWithOptionalProtocolLog(_cfg);
                imap.AuthenticationMechanisms.Remove("XOAUTH2");

                await imap.ConnectAsync(_cfg.ImapServer, _cfg.Port, SecureSocketOptions.SslOnConnect, ct);
                await imap.AuthenticateAsync(_cfg.Username, password, ct);

                var inbox = (IImapFolder)await imap.GetFolderAsync(_cfg.Folder, ct);
                await inbox.OpenAsync(FolderAccess.ReadWrite, ct);

                var since = DateTime.UtcNow.AddDays(-(_cfg.LookbackDays > 0 ? _cfg.LookbackDays : 2));
                SearchQuery query = SearchQuery.DeliveredAfter(since);
                if (!_cfg.ProcessSeen) query = query.And(SearchQuery.NotSeen);

                var ids = await inbox.SearchAsync(query, ct);
                if (ids.Count == 0)
                {
                    await imap.DisconnectAsync(true, ct);
                    return new EmailIngestionResult(0,0,0,0);
                }

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
                    ct.ThrowIfCancellationRequested();
                    messagesExamined++;

                    try
                    {
                        int? invoiceId = null;
                        MimeMessage? fullMsg = null;

                        // 1) Gmail thread id
                        var threadId = s.GMailThreadId;
                        if (threadId.HasValue && threadId.Value != 0UL)
                        {
                            var threadIdStr = threadId.Value.ToString("D", CultureInfo.InvariantCulture);
                            var invByThread = await invoiceRepo.GetByEmailThreadIdAsync(threadIdStr);
                            if (invByThread != null) invoiceId = invByThread.InvoiceId;
                        }

                        // 2) In-Reply-To / References
                        if (invoiceId == null)
                        {
                            try
                            {
                                fullMsg ??= await inbox.GetMessageAsync(s.UniqueId, ct);

                                static string Norm(string v) => v?.Trim().Trim('<', '>') ?? string.Empty;

                                var refsHeader = fullMsg.Headers["References"] ?? string.Empty;
                                var inReplyTo  = fullMsg.Headers["In-Reply-To"] ?? string.Empty;

                                var refIds = refsHeader
                                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(Norm)
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                                var irt = Norm(inReplyTo);
                                if (!string.IsNullOrEmpty(irt)) refIds.Add(irt);

                                if (refIds.Count > 0)
                                {
                                    var all = await invoiceRepo.GetAllAsync();
                                    var matched = all.FirstOrDefault(i =>
                                        !string.IsNullOrWhiteSpace(i.EmailMessageId) &&
                                        refIds.Contains(i.EmailMessageId.Trim('<', '>')));

                                    if (matched != null) invoiceId = matched.InvoiceId;
                                }
                            }
                            catch { /* header read optional */ }
                        }

                        // 3) Subject token #123
                        if (invoiceId == null)
                        {
                            var subj = s.Envelope?.Subject ?? string.Empty;
                            var m = subjectRegex.Match(subj);
                            if (m.Success && int.TryParse(m.Groups["id"].Value, out var parsed))
                                invoiceId = parsed;
                        }

                        if (invoiceId == null) continue; // no match
                        messagesMatched++;

                        // Optional To-filter
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
                            if (!allowed) continue;
                        }

                        fullMsg ??= await inbox.GetMessageAsync(s.UniqueId, ct);
                        var files = ExtractAllFiles(fullMsg, ct).ToList();

                        var anySavedForThisMsg = false;
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
                                }, ct);

                                filesSaved++;
                                receiptsCreated++;
                                anySavedForThisMsg = true;
                            }
                            catch
                            {
                                // log already in your original code if you want
                            }
                        }

                        if (anySavedForThisMsg)
                        {
                            await inbox.AddFlagsAsync(s.UniqueId, MessageFlags.Seen, true, ct);

                            if (!string.IsNullOrWhiteSpace(_cfg.ProcessedFolder))
                            {
                                try
                                {
                                    await MoveOrLabelProcessedAsync(imap, inbox, s.UniqueId, _cfg.ProcessedFolder!, ct);
                                }
                                catch { /* not fatal */ }
                            }
                        }
                    }
                    catch
                    {
                        // log per-message errors if desired
                    }
                }

                await imap.DisconnectAsync(true, ct);

                return new EmailIngestionResult(messagesExamined, messagesMatched, filesSaved, receiptsCreated);
            }
            finally
            {
                _gate.Release();
            }
        }

        // ========= helpers copied from your service =========

        private static ImapClient CreateImapClientWithOptionalProtocolLog(EmailIngestionSettings cfg)
        {
            if (!string.IsNullOrWhiteSpace(cfg.ProtocolLogDir))
            {
                var expanded = Environment.ExpandEnvironmentVariables(cfg.ProtocolLogDir);
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

        private string? ResolvePassword()
        {
            if (!string.IsNullOrWhiteSpace(_cfg.AppPassword))
                return _cfg.AppPassword;

            var env = Environment.GetEnvironmentVariable("EMAIL_INGESTION_PASSWORD");
            return string.IsNullOrWhiteSpace(env) ? null : env;
        }

        private static async Task MoveOrLabelProcessedAsync(ImapClient imap, IImapFolder inbox, UniqueId id, string processedPath, CancellationToken ct)
        {
            try
            {
                var processed = await EnsureFolderPathAsync(imap, processedPath, ct);
                await inbox.MoveToAsync(id, processed, ct);
            }
            catch
            {
                if (imap.Capabilities.HasFlag(ImapCapabilities.GMailExt1))
                    await inbox.AddLabelsAsync(id, new[] { processedPath }, true, ct);
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

        private static IEnumerable<(string FileName, byte[] Bytes)> ExtractAllFiles(MimeMessage msg, CancellationToken ct)
        {
            if (msg.Body == null) yield break;
            foreach (var r in ExtractFromEntity(msg.Body, ct))
                yield return r;
        }

        private static IEnumerable<(string FileName, byte[] Bytes)> ExtractFromEntity(MimeEntity entity, CancellationToken ct)
        {
            if (entity is MimePart part)
            {
                var isText = part.ContentType?.MediaType.Equals("text", StringComparison.OrdinalIgnoreCase) == true;
                var isHtmlOrPlain =
                    part.ContentType?.MediaSubtype.Equals("plain", StringComparison.OrdinalIgnoreCase) == true ||
                    part.ContentType?.MediaSubtype.Equals("html", StringComparison.OrdinalIgnoreCase) == true;

                var hasFileName = !string.IsNullOrWhiteSpace(part.FileName);

                if (isText && isHtmlOrPlain && !hasFileName)
                    yield break;

                using var ms = new MemoryStream();
                part.Content.DecodeTo(ms, ct);
                var bytes = ms.ToArray();

                var name = hasFileName ? part.FileName : GenerateFileNameFromContentType(part);
                if (bytes.Length > 0) yield return (name, bytes);
                yield break;
            }

            if (entity is TnefPart tnef)
            {
                foreach (var extracted in tnef.ExtractAttachments())
                    foreach (var r in ExtractFromEntity(extracted, ct)) yield return r;
                yield break;
            }

            if (entity is MessagePart msgPart && msgPart.Message != null)
            {
                foreach (var r in ExtractAllFiles(msgPart.Message, ct)) yield return r;
                yield break;
            }

            if (entity is Multipart mp)
            {
                foreach (var child in mp)
                    foreach (var r in ExtractFromEntity(child, ct)) yield return r;
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
    }
}
