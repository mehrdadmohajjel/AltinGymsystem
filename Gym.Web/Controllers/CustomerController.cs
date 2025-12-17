using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public CustomerController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public IActionResult Dashboard() => View();
    }
}
