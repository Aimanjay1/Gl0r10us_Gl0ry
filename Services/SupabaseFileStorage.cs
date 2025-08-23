using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using BizOpsAPI.Helpers;
using Microsoft.Extensions.Options;

// Import only the main Supabase namespace.
// Do NOT add: using Supabase.Storage;
using Supabase;

namespace BizOpsAPI.Services
{
    /// <summary>
    /// Saves files into Supabase Storage and returns either a public URL (if bucket is public)
    /// or a stable storage path "supabase://{bucket}/{objectPath}" you can later turn into a signed URL.
    /// </summary>
    public class SupabaseFileStorage : IFileStorage
    {
        private readonly Supabase.Client _supabase;   // fully qualified
        private readonly SupabaseSettings _cfg;

        public SupabaseFileStorage(Supabase.Client supabase, IOptions<SupabaseSettings> cfg)
        {
            _supabase = supabase ?? throw new ArgumentNullException(nameof(supabase));
            _cfg = (cfg ?? throw new ArgumentNullException(nameof(cfg))).Value;
        }

        public async Task<string> SaveAsync(byte[] bytes, string folder, string fileName, CancellationToken ct = default)
        {
            if (bytes is null || bytes.Length == 0)
                throw new ArgumentException("No file content provided.", nameof(bytes));

            folder ??= string.Empty;
            fileName ??= string.Empty;

            // sanitize file name
            var safeName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(safeName))
                safeName = $"file-{Guid.NewGuid():N}.bin";

            // object path in the bucket, e.g. "receipts/2025/08/20/20250820-...-invoice.pdf"
            var relDir = folder.Replace('\\', '/').Trim('/');
            var finalName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{safeName}";
            var objectPath = string.IsNullOrEmpty(relDir) ? finalName : $"{relDir}/{finalName}";

            // detect content type (very light heuristic)
            var contentType = GuessContentType(safeName);

            // Upload to Supabase Storage (byte[] overload)
            await _supabase.Storage
                .From(_cfg.Storage.Bucket)
                .Upload(
                    bytes,
                    objectPath,
                    new Supabase.Storage.FileOptions { ContentType = contentType, Upsert = false }
                );

            if (_cfg.Storage.PublicBucket)
            {
                // https://<project>.supabase.co/storage/v1/object/public/<bucket>/<objectPath>
                var publicUrl = _supabase.Storage
                    .From(_cfg.Storage.Bucket)
                    .GetPublicUrl(objectPath);

                return publicUrl;
            }
            else
            {
                // Stable internal URI to store in DB
                return $"supabase://{_cfg.Storage.Bucket}/{objectPath}";
            }
        }

        public async Task<bool> DeleteAsync(string urlOrPath, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(urlOrPath))
                    return false;

                // Accept either:
                // - "supabase://bucket/path/to/file.ext"
                // - Public URL "https://.../storage/v1/object/public/bucket/path/to/file.ext"
                // - Raw "bucket/path/to/file.ext" (assumed in configured bucket)
                string bucket = _cfg.Storage.Bucket;
                string objectPath;

                if (urlOrPath.StartsWith("supabase://", StringComparison.OrdinalIgnoreCase))
                {
                    var withoutScheme = urlOrPath.Substring("supabase://".Length);
                    var slash = withoutScheme.IndexOf('/');
                    if (slash <= 0) return false;
                    bucket = withoutScheme[..slash];
                    objectPath = withoutScheme[(slash + 1)..];
                }
                else if (urlOrPath.Contains("/storage/v1/object/", StringComparison.OrdinalIgnoreCase))
                {
                    // public URL form
                    // .../object/public/<bucket>/<objectPath>
                    var parts = new Uri(urlOrPath).AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    // [ "storage","v1","object","public", "<bucket>", "<...object...>" ]
                    if (parts.Length < 6) return false;
                    bucket = parts[4];
                    objectPath = string.Join('/', parts.Skip(5));
                }
                else
                {
                    // assume it's a raw object path in the configured bucket
                    objectPath = urlOrPath.TrimStart('/');
                }

                await _supabase.Storage
                    .From(bucket)
                    .Remove(new List<string> { objectPath });

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GuessContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => MediaTypeNames.Application.Pdf,
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".txt" => MediaTypeNames.Text.Plain,
                ".csv" => "text/csv",
                ".json" => MediaTypeNames.Application.Json,
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}
