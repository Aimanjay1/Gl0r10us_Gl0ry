using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BizOpsAPI.Helpers;
using Microsoft.Extensions.Options;
using Supabase;

namespace BizOpsAPI.Services
{
    /// <summary>
    /// Turns a stored storage reference (e.g., "supabase://bucket/path/to/file.pdf" or "path/to/file.pdf")
    /// into a downloadable URL. If the bucket is public, returns the public URL; otherwise generates a
    /// short-lived signed URL.
    /// </summary>
    public class ReceiptLinkService
    {
        private readonly Supabase.Client _supabase;
        private readonly SupabaseSettings _cfg;

        public ReceiptLinkService(Supabase.Client supabase, IOptions<SupabaseSettings> cfg)
        {
            _supabase = supabase ?? throw new ArgumentNullException(nameof(supabase));
            _cfg = (cfg ?? throw new ArgumentNullException(nameof(cfg))).Value;
        }

        /// <summary>
        /// Convert a stored path/URI to a downloadable URL (public or signed).
        /// </summary>
        /// <param name="storedUrlOrPath">
        /// - "supabase://bucket/objectPath"
        /// - raw "objectPath" (uses configured bucket)
        /// - public URL "https://.../storage/v1/object/public/bucket/objectPath"
        /// </param>
        /// <param name="ttlSeconds">Optional override for signed URL TTL (defaults to config).</param>
        public async Task<string> ToDownloadUrlAsync(
            string storedUrlOrPath,
            int? ttlSeconds = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(storedUrlOrPath))
                return string.Empty;

            ParseObjectRef(storedUrlOrPath, out var bucket, out var objectPath);

            if (_cfg.Storage.PublicBucket)
            {
                return _supabase.Storage
                           .From(bucket)
                           .GetPublicUrl(objectPath) ?? string.Empty;
            }

            var ttl = ttlSeconds ?? _cfg.Storage.SignedUrlTtlSeconds;

            // SDK does NOT accept CancellationToken here.
            var signed = await _supabase.Storage
                .From(bucket)
                .CreateSignedUrl(objectPath, ttl);

            return signed ?? string.Empty;
        }

        /// <summary>
        /// Batch version for convenience when listing many receipts.
        /// </summary>
        public async Task<IReadOnlyDictionary<string, string>> ToDownloadUrlsAsync(
            IEnumerable<string> storedList,
            int? ttlSeconds = null,
            CancellationToken ct = default)
        {
            var results = new ConcurrentDictionary<string, string>();
            var tasks = storedList
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(async s =>
                {
                    try
                    {
                        var url = await ToDownloadUrlAsync(s!, ttlSeconds, ct);
                        results[s!] = url;
                    }
                    catch
                    {
                        results[s!] = string.Empty;
                    }
                });

            await Task.WhenAll(tasks);
            return results;
        }

        /// <summary>
        /// Normalizes the input into (bucket, objectPath).
        /// Accepts:
        /// 1) supabase://bucket/objectPath
        /// 2) https://.../storage/v1/object/public/bucket/objectPath
        /// 3) raw objectPath (uses configured default bucket)
        /// </summary>
        private void ParseObjectRef(string input, out string bucket, out string objectPath)
        {
            bucket = _cfg.Storage.Bucket;
            objectPath = input.TrimStart('/');

            // Case 1: supabase://bucket/objectPath
            const string scheme = "supabase://";
            if (input.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
            {
                var without = input.Substring(scheme.Length);
                var slash = without.IndexOf('/');
                if (slash <= 0)
                    throw new ArgumentException("Invalid supabase URI. Expected supabase://bucket/path", nameof(input));

                bucket = without[..slash];
                objectPath = without[(slash + 1)..].TrimStart('/');
                return;
            }

            // Case 2: public URL variant
            // .../storage/v1/object/public/<bucket>/<objectPath>
            if (input.Contains("/storage/v1/object/", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(input, UriKind.Absolute);
                var parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                // ["storage","v1","object","public","<bucket>", "<...object...>"]
                if (parts.Length < 6)
                    throw new ArgumentException("Unrecognized storage public URL format.", nameof(input));

                bucket = parts[4];
                objectPath = string.Join('/', parts.Skip(5));
                return;
            }

            // Case 3: raw object path -> keep default bucket from config
            // objectPath already assigned above.
        }
    }
}
