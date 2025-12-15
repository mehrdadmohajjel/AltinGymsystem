using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTO.Wallet
{
    public class ManualChargeDto
    {
        public Guid UserId { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
    }
}
