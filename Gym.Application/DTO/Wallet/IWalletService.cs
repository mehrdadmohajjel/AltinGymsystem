using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTO.Wallet
{
    public interface IWalletService
    {
        Task<long> GetBalanceAsync(Guid userId);
        Task ManualChargeAsync(ManualChargeDto dto, Guid operatorUserId);
        Task UseBalanceAsync(Guid userId, long amount, string description);
    }
}
