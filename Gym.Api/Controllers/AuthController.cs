using Gym.Application.DTO.Auth;
using Gym.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            await _service.RegisterAsync(dto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            return Ok(await _service.LoginAsync(dto));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            return Ok(await _service.RefreshTokenAsync(refreshToken));
        }
    }
}
