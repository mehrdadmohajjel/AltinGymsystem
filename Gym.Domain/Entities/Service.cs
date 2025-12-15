using Gym.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Entities
{
    public class Service : BaseEntity
    {
        public Guid TenantId { get; set; }

        public string Title { get; set; }
        public ServiceType Type { get; set; }
        public long Price { get; set; }

        public int? SessionCount { get; set; }
        public int? DurationDays { get; set; }

        public Tenant Tenant { get; set; }
        public ICollection<UserService> UserServices { get; set; }
    }
}
