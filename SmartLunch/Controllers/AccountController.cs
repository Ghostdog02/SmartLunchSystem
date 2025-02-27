using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SmartLunch.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
            var userCreation = new UserCreation(HttpContext.RequestServices);

            if (claims == null)
            {
                throw new Exception($"Provided claims are null");
            }

            await userCreation.CreateUserIfNotExistingAsync(claims);

            //// Sign the user into the application's cookie scheme
            //var claimsIdentity = new ClaimsIdentity(
            //    claims,
            //    CookieAuthenticationDefaults.AuthenticationScheme // Use cookie auth type
            //);

            //await HttpContext.SignInAsync(
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    new ClaimsPrincipal(claimsIdentity),
            //    new AuthenticationProperties
            //    {
            //        IsPersistent = true // Optional: Keep the user logged in
            //    }
            //);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult LoginFailed()
        {
            return View();
        }

        public async Task<IActionResult> SignOut()
        {
            //// Sign out of the application's cookie
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign out of OpenID Connect (optional)
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
