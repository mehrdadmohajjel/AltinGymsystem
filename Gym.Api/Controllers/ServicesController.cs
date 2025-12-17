using Gym.Application.DTO.Services;
using Gym.Application.Services;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Api.Controllers
{
    [Authorize(Roles = "GymAdmin")]
    [ApiController]
    [Route("api/services")]
    public class ServicesController : BaseApiController
    {
        private readonly IServiceService _service;

        public ServicesController(IServiceService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "GymAdmin")]
        public async Task<IActionResult> Create(CreateServiceDto dto)
        {
            if (dto.Type == ServiceType.TimeBased && dto.DurationDays == null)
                return BadRequest("مدت زمان برای سرویس زمانی الزامی است");

            var service = new Service
            {
                Title = dto.Title,
                Type = dto.Type,
                Price = dto.Price,
                DurationDays = dto.DurationDays,
                TenantId = CurrentTenantId
            };

            await _service.CreateAsync(dto, CurrentTenantId);

            return Ok();
        }

        [HttpPost("assign")]
        public async Task<IActionResult> Assign(AssignServiceDto dto)
        {
            await _service.AssignToUserAsync(dto);
            return Ok();
        }

        [HttpPost("use")]
        [Authorize(Roles = "Employee,GymAdmin")]
        public async Task<IActionResult> Use(UseServiceDto dto)
        {
            await _service.UseServiceAsync(dto);
            return Ok();
        }
    }
}
