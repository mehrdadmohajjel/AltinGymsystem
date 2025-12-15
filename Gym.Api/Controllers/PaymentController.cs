using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parbad;

namespace Gym.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IOnlinePayment _payment;
        private readonly GymDbContext _db;

        public PaymentController(IOnlinePayment payment, GymDbContext db)
        {
            _payment = payment;
            _db = db;
        }

        [HttpPost("charge")]
        public async Task<IActionResult> Charge(long amount)
        {
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);
            var tenantId = Guid.Parse(User.FindFirst("tenantId")!.Value);

            var setting = await _db.TenantSettings
                .FirstAsync(x => x.TenantId == tenantId);

            var trackingNumber = Convert.ToInt64( Guid.NewGuid());

            // ذخیره درخواست پرداخت
            _db.PaymentRequests.Add(new PaymentRequest
            {
                UserId = userId,
                TenantId = tenantId,
                Amount = amount,
                TrackingNumber = trackingNumber.ToString(),
                IsPaid = false
            });

            await _db.SaveChangesAsync();

            var invoice = await _payment.RequestAsync(invoice =>
            {
                invoice
                    .SetAmount(amount)
                    .SetGateway("ZarinPal")
                    .SetCallbackUrl(setting.PaymentCallbackUrl)
                    .SetTrackingNumber(trackingNumber);
            });

            return Ok(invoice.GatewayTransporter.Descriptor.Url);
        }
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

            // شارژ کیف پول
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