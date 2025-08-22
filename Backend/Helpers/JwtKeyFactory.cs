using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BizOpsAPI.Helpers
{
    public static class JwtKeyFactory
    {
        /// <summary>
        /// If Secret is Base64 and decodes to ≥32 bytes, use it as-is.
        /// Otherwise treat it as a passphrase and derive a 32-byte key via PBKDF2(SHA256).
        /// </summary>
        public static SymmetricSecurityKey Create(JwtSettings jwt)
        {
            if (jwt is null) throw new ArgumentNullException(nameof(jwt));
            if (string.IsNullOrWhiteSpace(jwt.Secret))
                throw new InvalidOperationException("JwtSettings:Secret is missing.");

            // 1) Try Base64 raw key first
            try
            {
                var raw = Convert.FromBase64String(jwt.Secret);
                if (raw.Length >= 32) return new SymmetricSecurityKey(raw);
                // fall through if too short
            }
            catch
            {
                // not base64; treat as passphrase
            }

            // 2) Derive from passphrase using PBKDF2 with stable salt
            var saltMaterial = string.IsNullOrEmpty(jwt.KdfSalt) ? "BizOpsAPI.default-salt" : jwt.KdfSalt!;
            var saltBytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltMaterial));
            var iterations = Math.Max(jwt.KdfIterations, 100_000);

            using var kdf = new Rfc2898DeriveBytes(jwt.Secret, saltBytes, iterations, HashAlgorithmName.SHA256);
            var keyBytes = kdf.GetBytes(32); // 32 bytes = 256 bits (HS256)
            return new SymmetricSecurityKey(keyBytes);
        }
    }
}
