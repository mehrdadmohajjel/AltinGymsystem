using Microsoft.AspNetCore.Mvc;

namespace Gym.Web.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
