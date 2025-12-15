using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Enums
{
    public enum TransactionType
    {
        OnlineCharge = 1,
        ManualCharge = 2,
        ServiceUsage = 3,
        Refund = 4
    }
}
