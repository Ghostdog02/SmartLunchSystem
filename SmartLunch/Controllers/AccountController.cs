using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Api.Dtos;
using SmartLunch.Api.Mapping;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // This will:
            //  1) clear your local auth cookie
            //  2) send a sign‐out message to Google OIDC middleware
            var props = new AuthenticationProperties
            {
                // After sign‐out, redirect back home (or wherever you like)
                RedirectUri = Url.Action("Index", "Home")
            };

            return SignOut(
                props,
                CookieAuthenticationDefaults.AuthenticationScheme,
                GoogleOpenIdConnectDefaults.AuthenticationScheme
            );
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
            try
            {
                AuthenticateResult authenticateResult = await HttpContext.
                    AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded)
                {
                    throw new InvalidOperationException("Authentication failed.");
                }

                if (authenticateResult.Principal == null)
                {
                    throw new InvalidOperationException("No principal found in the authentication result.");
                }

                var claims = GetClaims(authenticateResult);
                ClaimsDto claimsDto = claims.ToClaimsDto()
                   ?? throw new InvalidOperationException("No valid claims found in the principal.");

                await CreateUser(claimsDto);

                

                return RedirectToAction("Index", "Home");
                //return Json(claimDto);
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

        private async Task CreateUser(ClaimsDto claimsDto)
        {
            var dto = claimsDto.ToUserCreationDto()
                ?? throw new InvalidOperationException("Failed to convert claims to UserCreationDto.");

            
            // var userCreation = new UserCreation(serviceProvider);
            // await userCreation.CreateUserIfNotExistingAsync(claimsDto);
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
