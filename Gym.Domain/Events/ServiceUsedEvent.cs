using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain.Events
{
    public class ServiceUsedEvent : IDomainEvent
    {
        public Guid UserId { get; set; }
        public string ServiceTitle { get; set; }
    }

}
