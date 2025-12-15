using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Api.BackgroundJobs
{
    public class ServiceExpirationJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ServiceExpirationJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();

                var expiredServices = await db.UserServices
                    .Where(x =>
                        x.IsActive &&
                        x.ExpireAt != null &&
                        x.ExpireAt < DateTime.UtcNow)
                    .ToListAsync();

                foreach (var us in expiredServices)
                    us.IsActive = false;

                await db.SaveChangesAsync();

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }

}
