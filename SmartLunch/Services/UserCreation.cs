using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Api.Dtos;
using SmartLunch.Database;

namespace SmartLunch.Services
{
    public class UserCreation(IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory)
    {
        private IServiceProvider ServiceProvider { get; } = serviceProvider;

        public IHttpClientFactory HttpClientFactory { get; } = httpClientFactory;

        public async Task CreateUserIfNotExistingAsync(ClaimsDto claimsDto)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();

                var client = HttpClientFactory.CreateClient("UserManagement_Api_Client");

                SmartLunchDbContext context = scope.ServiceProvider.
                    GetRequiredService<SmartLunchDbContext>();

                UserManager<User> userManager = scope.ServiceProvider.
                    GetRequiredService<UserManager<User>>();



                string email = claimsDto.Email!;

                // User? currentUser = await userManager.FindByEmailAsync(email);
                // User user = await client.GetAsync<User>($"api/users/email/{email}");    
                var getResponse = await client.GetAsync($"api/userManagement/{email}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    var userData = new UserCreationDto(0,
                                                       email,
                                                       DateTime.Now,
                                                       claimsDto.FullName,
                                                       claimsDto.PhoneNumber);

                    var postResponse = await client.PostAsJsonAsync($"api/userManagement",
                        userData);

                    if (!postResponse.IsSuccessStatusCode)
                    {
                        throw postResponse.StatusCode switch
                        {
                            HttpStatusCode.BadRequest =>
                                new ArgumentException("Invalid user data provided"),

                            HttpStatusCode.Conflict =>
                                new InvalidOperationException("User already exists"),

                            HttpStatusCode.Unauthorized =>
                                new UnauthorizedAccessException("Authentication required"),

                            _ => new HttpRequestException($"Request failed: {postResponse.StatusCode}"),
                        };
                    }
                    
                    else
                    {
                        
                    }
                }

                else
                {
                    User? user = await getResponse.Content.ReadFromJsonAsync<User>();

                    if (user == null)
                    {
                        throw new InvalidOperationException(
                            $"Failed to deserialize user data from response");
                    }

                    user = await CreateUserAsync(userManager, claimsDto);
                    await AddRoleToUser(userManager, currentUser);
                }

                UpdateLoginDate(currentUser);

                await context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while saving data.", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during finding user by email.", ex);
            }
        }

        private static async Task<User> CreateUserAsync(UserManager<User> userManager, 
            ClaimsDto claimsDto)
        {
            string firstName = claimsDto.FirstName!;
            string email = claimsDto.Email!;

            var user = new User
            {
                UserName = firstName,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = false,
                RegistrationDate = DateTime.Now
            };

            IdentityResult creationResult = await userManager.CreateAsync(user);

            var creationValidation = new IdentityResultValidator();
            creationValidation.CheckSuccess(creationResult, "User creation failed");

            return user;
        }

        private static async Task AddRoleToUser(UserManager<User> userManager, User user)
        {
            IdentityResult result = await userManager.AddToRoleAsync(user, "Parent");

            var resultValidation = new IdentityResultValidator();
            resultValidation.CheckSuccess(result, "Role assignment failed");
        }

        // private static void UpdateLoginDate(User user)
        // {
        //     var today = DateTime.Now;
        //     user.LastLoginDate = today;
        // }
    }
}
