using Gym.Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    }
}
