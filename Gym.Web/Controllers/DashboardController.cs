using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
