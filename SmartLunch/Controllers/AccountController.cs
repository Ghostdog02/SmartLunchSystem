using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SmartLunch.Services;

namespace SmartLunch.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider = "Google")
        {
            // Prepare the authentication properties
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            // Challenge the specified authentication scheme
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // Get the external login info
            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.
                AuthenticationScheme);

            if (authenticateResult?.Succeeded != true)
            {
                return RedirectToAction("LoginFailed");
            }

            // Extract user claims
            var claims = authenticateResult.Principal.Claims;

            var usersCreation = new UsersCreation(HttpContext.RequestServices);
            await usersCreation.CreateUsersAsync(claims);

            // Here you would typically:
            // 1. Check if user exists in your system
            // 2. Create user if not exists
            // 3. Sign in the user

            return RedirectToAction("Index", "Home");
        }

        public IActionResult LoginFailed()
        {
            return View();
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
