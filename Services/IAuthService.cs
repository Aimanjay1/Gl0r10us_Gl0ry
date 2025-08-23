using BizOpsAPI.DTOs;

namespace BizOpsAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(CreateUserDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
