using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Entities
{
    public class TenantSetting : BaseEntity
    {
        public Guid TenantId { get; set; }

        // Payment
        public string MerchantId { get; set; }
        public string PaymentCallbackUrl { get; set; }

        // SMS
        public string KavenegarApiKey { get; set; }
        public long LowBalanceThreshold { get; set; }

        public Tenant Tenant { get; set; }
    }
}
