using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
