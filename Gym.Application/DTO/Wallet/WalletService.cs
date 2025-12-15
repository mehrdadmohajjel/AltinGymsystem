using Gym.Application.Events;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Domain.Events;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTO.Wallet
{
    public class WalletService : IWalletService
    {
        private readonly GymDbContext _db;

        private readonly IDomainEventDispatcher _dispatcher;

        public WalletService(
            GymDbContext db,
            IDomainEventDispatcher dispatcher)
        {
            _db = db;
            _dispatcher = dispatcher;
        }

        public async Task<long> GetBalanceAsync(Guid userId)
        {
            var wallet = await _db.Wallets.FirstAsync(x => x.UserId == userId);
            return wallet.Balance;
        }

        public async Task ManualChargeAsync(ManualChargeDto dto, Guid operatorUserId)
        {
            var wallet = await _db.Wallets.FirstAsync(x => x.UserId == dto.UserId);

            wallet.Balance += dto.Amount;

            _db.Transactions.Add(new Transaction
            {
                WalletId = wallet.Id,
                Amount = dto.Amount,
                Type = TransactionType.ManualCharge,
                Description = dto.Description
            });

            await _db.SaveChangesAsync();
        }
        public async Task UseBalanceAsync(Guid userId, long amount, string description)
        {
            var wallet = await _db.Wallets
                .Include(x => x.User)
                .ThenInclude(x => x.Tenant)
                .ThenInclude(x => x.Setting)
                .FirstAsync(x => x.UserId == userId);

            if (wallet.Balance < amount)
                throw new Exception("موجودی کیف پول کافی نیست");

            // کسر موجودی
            wallet.Balance -= amount;

            _db.Transactions.Add(new Transaction
            {
                WalletId = wallet.Id,
                Amount = -amount,
                Type = TransactionType.ServiceUsage,
                Description = description
            });

            await _db.SaveChangesAsync();

            // 🔔 بررسی حداقل موجودی (بعد از کم شدن)
            var threshold = wallet.User.Tenant.Setting.LowBalanceThreshold;

            if (wallet.Balance <= threshold)
            {
                await _dispatcher.DispatchAsync(new LowBalanceEvent
                {
                    UserId = userId,
                    Balance = wallet.Balance
                });
            }
        }


    }
}
