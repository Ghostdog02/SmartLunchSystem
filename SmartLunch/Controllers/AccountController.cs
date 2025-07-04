using System.Security.Claims;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Api.Dtos;
using SmartLunch.Api.Mapping;
using SmartLunch.Services;

namespace SmartLunch.Controllers
{

    public class AccountController(IServiceProvider serviceProvider,
                             IHttpClientFactory httpClientFactory) : Controller
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            string redirectUri = Url.Action("Index", "Home");
            return Redirect(redirectUri);
        }

        [AllowAnonymous]
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties() { RedirectUri = Url.Action("GoogleResponse") }
            );
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(
                    GoogleDefaults.AuthenticationScheme
                );

                if (!authenticateResult.Succeeded)
                {
                    throw new InvalidOperationException("Authentication failed.");
                }

                if (authenticateResult.Principal == null)
                {
                    throw new InvalidOperationException(
                        "No principal found in the authentication result."
                    );
                }

                var claims = GetClaims(authenticateResult);
                ClaimsDto claimsDto =
                    claims.ToClaimsDto()
                    ?? throw new InvalidOperationException(
                        "No valid claims found in the principal."
                    );

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
            // HttpClient client = _serviceProvider.GetRequiredService<HttpClient>();

            var dto =
                claimsDto.ToUserCreationDto()
                ?? throw new InvalidOperationException(
                    "Failed to convert claims to UserCreationDto."
                );

            var userCreation = new UserCreation(_serviceProvider, _httpClientFactory);
            await userCreation.CreateUserIfNotExistingAsync(dto);
        }

        public IEnumerable<Claim> GetClaims(AuthenticateResult authenticateResult)
        {
            ClaimsIdentity identity =
                authenticateResult.Principal!.Identities.FirstOrDefault(i => i.Claims.Any())
                ?? throw new ArgumentException("No valid claims found in the principal.");

            return identity.Claims;
        }
    }
}
