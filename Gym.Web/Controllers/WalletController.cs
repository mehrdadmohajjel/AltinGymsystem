using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    public class WalletController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
