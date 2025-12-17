using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class WalletController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public WalletController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public IActionResult Index() => View();
    }
}
