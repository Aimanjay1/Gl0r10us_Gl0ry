using BizOpsAPI.Repositories;
using BizOpsAPI.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using BizOpsAPI.Helpers;
using BizOpsAPI.DTOs;

namespace BizOpsAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly JwtSettings _jwt;

        public AuthService(IUserRepository users, IOptions<JwtSettings> jwtOptions)
        {
            _users = users;
            _jwt = jwtOptions.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(CreateUserDto dto)
        {
            var existing = await _users.GetByEmailAsync(dto.Email);
            if (existing != null) throw new InvalidOperationException("Email already exists.");

            var user = new User
            {
                FullName = dto.FullName,
                CompanyName = dto.CompanyName,
                CompanyAddress = dto.CompanyAddress,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber,
                LogoUrl = dto.LogoUrl,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                Clients = new List<Client>(),
                Invoices = new List<Invoice>(),
                Expenses = new List<Expense>(),
                Revenues = new List<Revenue>()
            };

            await _users.CreateAsync(user);

            var (token, expiresAt) = GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAt,
                User = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    CompanyName = user.CompanyName,
                    Email = user.Email,
                    LogoUrl = user.LogoUrl
                }
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok) return null;

            var (token, expiresAt) = GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAt,
                User = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    CompanyName = user.CompanyName,
                    Email = user.Email,
                    LogoUrl = user.LogoUrl
                }
            };
        }

        private (string token, DateTime expiresAtUtc) GenerateToken(User user)
        {
            var key   = JwtKeyFactory.Create(_jwt);                 // ✅ robust key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullname", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwt, expiresAt);
        }
    }
}
