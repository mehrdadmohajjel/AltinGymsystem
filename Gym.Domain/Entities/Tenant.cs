using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string SubDomain { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Service> Services { get; set; }
        public TenantSetting Setting { get; set; }
    }
}
