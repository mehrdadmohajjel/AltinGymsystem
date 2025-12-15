using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
