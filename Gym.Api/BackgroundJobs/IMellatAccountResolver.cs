using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Parbad.Gateway.Mellat;

namespace Gym.Api.BackgroundJobs
{
    public interface IMellatAccountResolver
    {
        Task<MellatGatewayAccount> ResolveAsync(Guid tenantId);
    }

    public class MellatAccountResolver : IMellatAccountResolver
    {
        private readonly GymDbContext _db;

        public MellatAccountResolver(GymDbContext db)
        {
            _db = db;
        }

        public async Task<MellatGatewayAccount> ResolveAsync(Guid tenantId)
        {
            var setting = await _db.TenantSettings
                .FirstAsync(x => x.TenantId == tenantId);

            return new MellatGatewayAccount
            {
                TerminalId = setting.MellatTerminalId,
                UserName = setting.MellatUserName,
                UserPassword = setting.MellatUserPassword
            };
        }
    }

}
