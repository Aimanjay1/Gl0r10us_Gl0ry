using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BizOpsAPI.Helpers
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var sub = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) 
                          ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
                return int.TryParse(sub, out var id) ? id : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        public string? FullName => _httpContextAccessor.HttpContext?.User.FindFirst("fullname")?.Value;
    }
}
