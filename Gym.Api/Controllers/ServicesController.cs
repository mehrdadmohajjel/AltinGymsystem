using Gym.Application.DTO.Services;
using Gym.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Api.Controllers
{
    [Authorize(Roles = "GymAdmin")]
    [ApiController]
    [Route("api/services")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServicesController(IServiceService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceDto dto)
        {
            var tenantId = Guid.Parse(User.FindFirst("tenantId")!.Value);
            await _service.CreateAsync(dto, tenantId);
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
