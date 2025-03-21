using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Services;
using System.Security.Authentication;
using System.Security.Claims;

namespace SmartLunch.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        //public readonly string googleUserInfoUrl;
        private readonly IServiceProvider serviceProvider;

        public AccountController(IServiceProvider serviceProvider)
        {
            //this.googleUserInfoUrl = googleUserInfoUrl;
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

            //IEnumerable<Claim>? claims = authenticateResult.Principal?.Claims;

            var userCreation = new UsersCreation(serviceProvider);

            //await userCreation.CreateUserIfNotExistingAsync(claims);

            return Json(claims);
        }

        private async Task<AuthenticateResult> AuthenticateResultAsync()
        {
            try
            {
                //CookieAuthenticationDefaults.AuthenticationScheme
                AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

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

            catch (AuthenticationException ex)
            {
                throw new Exception("An authentication error occurred.", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during authentication.", ex);
            }
        }

        public IEnumerable<Claim> GetClaims(AuthenticateResult authenticateResult)
        {
            try
            {
                ClaimsIdentity identity = authenticateResult.Principal!.Identities.FirstOrDefault(i => i.Claims.Any());

                var claims = identity.Claims.ToList();

                //var claims = authenticateResult.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
                //{
                //    claim.Issuer,
                //    claim.OriginalIssuer,
                //    claim.Type,
                //    claim.Value
                //});

                return claims;
            }

            catch (NullReferenceException)
            {
                throw new Exception("No claims found in the principal.");
            }

            catch (InvalidOperationException)
            {
                throw new Exception("An invalid operation occurred during claim retrieval.");
            }

            catch (Exception)
            {
                throw new Exception("An unexpected error occurred during claim retrieval.");
            }
        }
    }
}
