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

        private IEnumerable<Claim> GetClaims(AuthenticateResult authenticateResult)
        {
            try
            {
                var claims = authenticateResult.Principal!.Identities.FirstOrDefault()!.Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

                return (IEnumerable<Claim>)claims;
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

        //public async Task<TokenDto> AuthenticateWithGoogleAsync(string accessToken, string idToken)
        //{
        //    // Validate the ID token and retrieve the payload
        //    var payload = await ValidateGoogleTokenAsync(idToken);

        //    // Fetch additional user profile information using the access token
        //    var userProfile = await GetGoogleUserProfileAsync(accessToken);

        //    //// Check if the user already exists in the database
        //    //var userExists = await identityUserRepository.IsExistsAsync(payload.Email);

        //    //// If user doesn't exist, create a new user
        //    //if (!userExists)
        //    //    await CreateNewUserAsync(userProfile);

        //    // Prepare the sign-in DTO for login
        //    var signInDto = new SignInDto(
        //        Email: userProfile.Email,
        //        Password: string.Empty,  // No password for external logins
        //        SignUpMethod: SignUpMethod.External
        //    );

        //    // Use the login service to issue a token
        //    var token = await connectService.LoginAsync(signInDto);

        //    return token;
        //}

        //private async Task<GoogleUserProfileViewModel> GetGoogleUserProfileAsync(string accessToken)
        //{
        //    using var httpClient = new HttpClient();

        //    // Send GET request to Google UserInfo API
        //    var response = await httpClient.GetAsync($"{googleUserInfoUrl}?access_token={accessToken}");
        //    response.EnsureSuccessStatusCode();

        //    // Parse the JSON response
        //    var content = await response.Content.ReadAsStringAsync();
        //    var userProfile = JsonConvert.DeserializeObject<GoogleUserProfileViewModel>(content);

        //    return userProfile!;
        //}

        //private static async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
        //{
        //    var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

        //    if (payload == null || string.IsNullOrEmpty(payload.Email))
        //        throw new UnauthorizedAccessException("Invalid Google Token");

        //    return payload;
        //}
    }
}
