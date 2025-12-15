using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Entities
{
    public class PaymentRequest : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }

        public long Amount { get; set; }
        public string TrackingNumber { get; set; }
        public bool IsPaid { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
