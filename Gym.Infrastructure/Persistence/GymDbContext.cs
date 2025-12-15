using Gym.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Gym.Infrastructure.Persistence
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options)
            : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSetting> TenantSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Domain.Entities.Transaction> Transactions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<UserService> UserServices { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.NationalCode)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(x => x.Wallet)
                .WithOne(x => x.User)
                .HasForeignKey<Wallet>(x => x.UserId);

            modelBuilder.Entity<Tenant>()
                .HasOne(x => x.Setting)
                .WithOne(x => x.Tenant)
                .HasForeignKey<TenantSetting>(x => x.TenantId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
