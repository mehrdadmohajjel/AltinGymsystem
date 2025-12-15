using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Entities
{
    public class UserService : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }

        public int RemainingSessions { get; set; }
        public DateTime? ExpireAt { get; set; }
        public bool IsActive { get; set; }

        public User User { get; set; }
        public Service Service { get; set; }
    }
}
