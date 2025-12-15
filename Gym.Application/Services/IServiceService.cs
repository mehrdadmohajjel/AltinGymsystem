using Gym.Application.DTO.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Services
{
    public interface IServiceService
    {
        Task CreateAsync(CreateServiceDto dto, Guid tenantId);
        Task AssignToUserAsync(AssignServiceDto dto);
        Task UseServiceAsync(UseServiceDto dto);
    }
}
