using Gym.Application.Notifications;
using Gym.Domain.Events;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly INotificationService _notification;
        private readonly GymDbContext _db;

        public DomainEventDispatcher(
            INotificationService notification,
            GymDbContext db)
        {
            _notification = notification;
            _db = db;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case ServiceUsedEvent e:
                    await HandleServiceUsed(e);
                    break;

                case LowBalanceEvent e:
                    await HandleLowBalance(e);
                    break;
            }
        }

        private async Task HandleServiceUsed(ServiceUsedEvent e)
        {
            var user = await _db.Users.FindAsync(e.UserId);
            if (user == null) return;

            await _notification.SendSmsAsync(
                user.Mobile,
                $"کاربر گرامی، سرویس «{e.ServiceTitle}» برای شما استفاده شد."
            );
        }

        private async Task HandleLowBalance(LowBalanceEvent e)
        {
            var user = await _db.Users
                .Include(x => x.Tenant)
                .ThenInclude(x => x.Setting)
                .FirstAsync(x => x.Id == e.UserId);

            var threshold = user.Tenant.Setting.LowBalanceThreshold;
            if (e.Balance > threshold) return;

            await _notification.SendSmsAsync(
                user.Mobile,
                $"هشدار: موجودی کیف پول شما {e.Balance:N0} ریال است."
            );
        }
    }
}
