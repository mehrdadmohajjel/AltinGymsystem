using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Gym.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public AuthController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string nationalCode, string password)
        {
            var client = _factory.CreateClient();

            var content = new StringContent(
                JsonSerializer.Serialize(new { nationalCode, password }),
                Encoding.UTF8,
                "application/json");

            var res = await client.PostAsync("https://localhost:5001/api/auth/login", content);

            if (!res.IsSuccessStatusCode)
                return View();

            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            HttpContext.Session.SetString("token", doc.RootElement.GetProperty("accessToken").GetString());
            HttpContext.Session.SetString("role", doc.RootElement.GetProperty("role").GetString());

            return RedirectToAction("Index", "Dashboard");
        }
    }
}