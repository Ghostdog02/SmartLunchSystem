using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Services;
using System.Security.Claims;

namespace SmartLunch.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IServiceProvider serviceProvider;

        public AccountController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            AuthenticateResult authenticateResult = await AuthenticateResultAsync();

            var claims = GetClaims(authenticateResult);

            CreateUsers(claims).Wait();

            return Json(claims);
        }

        private async Task CreateUsers(IEnumerable<Claim> claims)
        {
            var userCreation = new UsersCreation(serviceProvider);
            await userCreation.CreateUserIfNotExistingAsync(claims);
        }

        private async Task<AuthenticateResult> AuthenticateResultAsync()
        {
            try
            {
                AuthenticateResult authenticateResult = await HttpContext.
                    AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded)
                {
                    throw new Exception("Authentication has failed.", authenticateResult.Failure);
                }

                return authenticateResult;
            }

            catch (InvalidOperationException ex)
            {
                throw new Exception("An invalid operation occurred during authentication.", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during authentication.", ex);
            }
        }

        public IEnumerable<Claim> GetClaims(AuthenticateResult authenticateResult)
        {
            ClaimsIdentity identity = authenticateResult.Principal!.Identities.
                FirstOrDefault(i => i.Claims.Any())
                ?? throw new ArgumentException("No valid claims found in the principal.");

            return identity.Claims;
        }
    }
}
