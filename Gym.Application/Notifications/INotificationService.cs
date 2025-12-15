using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Notifications
{
    public interface INotificationService
    {
        Task SendSmsAsync(string mobile, string message);
        Task SendWhatsAppAsync(string mobile, string message);
    }

}
