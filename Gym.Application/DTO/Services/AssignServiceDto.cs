using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.DTO.Services
{
    public class AssignServiceDto
    {
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
    }
}
