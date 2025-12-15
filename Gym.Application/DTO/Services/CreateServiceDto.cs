using Gym.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTO.Services
{
    public class CreateServiceDto
    {
        public string Title { get; set; }
        public ServiceType Type { get; set; }
        public long Price { get; set; }

        public int? SessionCount { get; set; }
        public int? DurationDays { get; set; }
    }
}
