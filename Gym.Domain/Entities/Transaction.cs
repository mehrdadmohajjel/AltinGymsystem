using Gym.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid WalletId { get; set; }
        public long Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; }

        public Wallet Wallet { get; set; }
    }
}
