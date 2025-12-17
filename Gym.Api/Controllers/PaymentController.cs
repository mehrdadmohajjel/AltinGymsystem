using Gym.Api.BackgroundJobs;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parbad;
using Parbad.Builder;
using Parbad.Gateway.Mellat;


namespace Gym.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : BaseApiController
    {
        private readonly IOnlinePayment _payment;
        private readonly GymDbContext _db;
        private readonly IMellatAccountResolver _resolver;

        public PaymentController(IOnlinePayment payment, GymDbContext db, IMellatAccountResolver resolver)
        {
            _payment = payment;
            _resolver = resolver;

            _db = db;
        }

        // -------------------- Charge Request --------------------

        [HttpPost("charge")]
        public async Task<IActionResult> Charge(long amount)
        {
            var setting = await _db.TenantSettings
                .FirstAsync(x => x.TenantId == CurrentTenantId);

            var trackingNumber = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            _db.PaymentRequests.Add(new PaymentRequest
            {
                UserId = CurrentUserId,
                TenantId = CurrentTenantId,
                Amount = amount,
                TrackingNumber = trackingNumber.ToString(),
                IsPaid = false
            });

            await _db.SaveChangesAsync();

            var result = await _payment.RequestAsync(invoice =>
            {
                invoice
                    .SetAmount(amount)
                    .SetGateway(MellatGateway.Name)
                    .SetTrackingNumber(trackingNumber)
                    .SetCallbackUrl(setting.PaymentCallbackUrl)
                    .UseAccount("MellatAccount");
  
            });

            return Ok(result.GatewayTransporter.Descriptor.Url);
        }

        // -------------------- Callback --------------------
        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var fetchResult = await _payment.FetchAsync();

            if (!fetchResult.IsSucceed)
                return BadRequest("پرداخت ناموفق");

            var verifyResult = await _payment.VerifyAsync(fetchResult);

            if (!verifyResult.IsSucceed)
                return BadRequest("تأیید پرداخت ناموفق");

            var trackingNumber = fetchResult.TrackingNumber;
            var amount = fetchResult.Amount;

            var paymentRequest = await _db.PaymentRequests
                .FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber.ToString());

            if (paymentRequest == null || paymentRequest.IsPaid)
                return BadRequest("درخواست پرداخت نامعتبر است");

            var wallet = await _db.Wallets
                .FirstAsync(x => x.UserId == paymentRequest.UserId);

            wallet.Balance += amount;

            _db.Transactions.Add(new Transaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = TransactionType.OnlineCharge,
                Description = "شارژ آنلاین کیف پول"
            });

            paymentRequest.IsPaid = true;
            paymentRequest.PaidAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok("پرداخت با موفقیت انجام شد");
        }
    }

}