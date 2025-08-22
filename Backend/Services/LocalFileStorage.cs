// Services/LocalFileStorage.cs
using Microsoft.AspNetCore.Hosting;

namespace BizOpsAPI.Services
{
    /// <summary>
    /// Saves files under wwwroot/{folder}/{fileName} and returns a web path like /receipts/{fileName}
    /// </summary>
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _webRoot;

        public LocalFileStorage(IWebHostEnvironment env)
        {
            // Ensure we have a web root (wwwroot). If null, fallback near the app.
            _webRoot = env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            Directory.CreateDirectory(_webRoot);
        }

        public async Task<string> SaveAsync(byte[] bytes, string folder, string fileName, CancellationToken ct = default)
        {
            // sanitize file name
            var safeName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(safeName)) safeName = $"file-{Guid.NewGuid():N}.bin";

            // ensure folder exists
            var relDir = folder.Replace('\\', '/').Trim('/');        // e.g. "receipts"
            var absDir = Path.Combine(_webRoot, relDir);
            Directory.CreateDirectory(absDir);

            // unique file name to avoid clashes
            var finalName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{safeName}";
            var absPath   = Path.Combine(absDir, finalName);

            await File.WriteAllBytesAsync(absPath, bytes, ct);

            // return a web path your app can serve (StaticFiles)
            var urlPath = "/" + Path.Combine(relDir, finalName).Replace('\\', '/'); // e.g. /receipts/20250817-...-doc.pdf
            return urlPath;
        }

        public Task<bool> DeleteAsync(string urlOrPath, CancellationToken ct = default)
        {
            try
            {
                // accept either a web path "/receipts/..." or an absolute path
                string absPath = urlOrPath.StartsWith("/")
                    ? Path.Combine(_webRoot, urlOrPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))
                    : urlOrPath;

                if (File.Exists(absPath))
                {
                    File.Delete(absPath);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
