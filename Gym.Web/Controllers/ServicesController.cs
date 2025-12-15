using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
