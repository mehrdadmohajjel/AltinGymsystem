using Gym.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid TenantId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        public Wallet Wallet { get; set; }
        public ICollection<UserService> UserServices { get; set; }

        public Tenant Tenant { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
