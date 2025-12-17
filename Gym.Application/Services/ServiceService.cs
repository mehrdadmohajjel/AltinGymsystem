using Gym.Application.DTO.Services;
using Gym.Application.DTO.Wallet;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly GymDbContext _db;
        private readonly IWalletService _walletService;

        public ServiceService(GymDbContext db, IWalletService walletService)
        {
            _db = db;
            _walletService = walletService;
        }

        // 1️⃣ تعریف خدمت
        public async Task CreateAsync(CreateServiceDto dto, Guid tenantId)
        {
            if (dto.Type == ServiceType.SessionBased )
                throw new Exception("تعداد جلسات الزامی است");

            if (dto.Type == ServiceType.TimeBased && dto.DurationDays == null)
                throw new Exception("مدت زمان الزامی است");

            _db.Services.Add(new Service
            {
                TenantId = tenantId,
                Title = dto.Title,
                Type = dto.Type,
                Price = dto.Price,
                DurationDays = dto.DurationDays,
            });

            await _db.SaveChangesAsync();
        }

        // 2️⃣ تخصیص خدمت به کاربر
        public async Task AssignToUserAsync(AssignServiceDto dto)
        {
            var service = await _db.Services.FirstAsync(x => x.Id == dto.ServiceId);

            // کسر مبلغ
            await _walletService.UseBalanceAsync(
                dto.UserId,
                service.Price,
                $"فعال‌سازی سرویس {service.Title}"
            );

            var userService = new UserService
            {
                UserId = dto.UserId,
                ServiceId = dto.ServiceId,
                IsActive = true
            };

            if (service.Type == ServiceType.SessionBased)
            {
                userService.RemainingSessions = service.SessionCount!.Value;
            }
            else
            {
                userService.ExpireAt =
                    DateTime.UtcNow.AddDays(service.DurationDays!.Value);
            }

            _db.UserServices.Add(userService);
            await _db.SaveChangesAsync();
        }

        // 3️⃣ استفاده از خدمت
        public async Task UseServiceAsync(UseServiceDto dto)
        {
            var userService = await _db.UserServices
                .Include(x => x.Service)
                .FirstOrDefaultAsync(x => x.Id == dto.UserServiceId)
                ?? throw new Exception("سرویس کاربر یافت نشد");

            if (userService.Service.Type != ServiceType.SessionBased)
                throw new Exception("این سرویس جلسه‌ای نیست");

            if (userService.RemainingSessions <= 0)
                throw new Exception("جلسه‌ای باقی نمانده");

            userService.RemainingSessions--;

            await _db.SaveChangesAsync();
        }
    }
}
