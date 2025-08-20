using System.Security.Claims;
using BizOpsAPI.DTOs;
using BizOpsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizOpsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] CreateUserDto dto)
        {
            var res = await _auth.RegisterAsync(dto);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var res = await _auth.LoginAsync(dto);
            if (res == null) return Unauthorized(new { message = "Invalid credentials" });
            return Ok(res);
        }

        [Authorize]
        [HttpGet("me")]
        public ActionResult<object> Me()
        {
            return Ok(new
            {
                sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value,
                email = User.FindFirstValue(ClaimTypes.Email),
                name = User.FindFirst("fullname")?.Value
            });
        }
    }
}
