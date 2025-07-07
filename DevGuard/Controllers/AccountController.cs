using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevGuard.Api.Dtos;
using DevGuard.Api.Mapping;
using DevGuard.Services;

namespace DevGuard.Controllers
{
    public class AccountController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(
            IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory
        )
        {
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            string redirectUri = Url.Action("Index", "Home")!;
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

                var client = _httpClientFactory.CreateClient("RoleManagementAPI");

                var responseMessage = await client.GetAsync($"api/roleManagement/{claimsDto.Email}");

                if (responseMessage.IsSuccessStatusCode)
                {
                    string? role = await responseMessage.Content.ReadFromJsonAsync<string>();

                    if (role == null || role == string.Empty)
                    {
                        throw new InvalidOperationException($"Getting role by email failed");
                    }

                    var localClaims = new List<Claim>
                    {
                        new(ClaimTypes.Name, claimsDto.FullName),
                        new(ClaimTypes.Email, claimsDto.Email),
                        new(ClaimTypes.Role, role)
                    };

                    var identity = new ClaimsIdentity(localClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                }

                return RedirectToAction("Index", "Home");
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
