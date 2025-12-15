using Gym.Domain.Entities;

namespace Gym.Application.Security
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(Guid userId);
    }
}
