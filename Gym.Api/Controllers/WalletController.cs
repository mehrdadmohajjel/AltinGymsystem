using Gym.Application.DTO.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Api.Controllers
{
    [Authorize(Roles = "GymAdmin,Employee")]
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpPost("manual-charge")]
        public async Task<IActionResult> ManualCharge(ManualChargeDto dto)
        {
            var operatorId = Guid.Parse(User.FindFirst("sub")!.Value);
            await _service.ManualChargeAsync(dto, operatorId);
            return Ok();
        }
    }
}
