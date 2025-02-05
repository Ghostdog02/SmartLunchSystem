using Microsoft.AspNetCore.Mvc;

namespace SmartLunch.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
