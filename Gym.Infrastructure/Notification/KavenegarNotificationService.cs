using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Notification
{
    public class KavenegarNotificationService : INotificationService
    {
        private readonly IConfiguration _config;

        public KavenegarNotificationService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendSmsAsync(string mobile, string message)
        {
            var apiKey = _config["Kavenegar:ApiKey"];
            var sender = _config["Kavenegar:Sender"];

            var api = new KavenegarApi(apiKey);
            await api.Send(sender, mobile, message);
        }

        public Task SendWhatsAppAsync(string mobile, string message)
        {
            // Placeholder – Future Provider
            return Task.CompletedTask;
        }
    }
}
