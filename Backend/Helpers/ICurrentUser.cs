using System.Security.Claims;

namespace BizOpsAPI.Helpers
{
    public interface ICurrentUser
    {
        int? UserId { get; }
        string? Email { get; }
        string? FullName { get; }
    }
}
