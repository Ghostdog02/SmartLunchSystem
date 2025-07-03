using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Api.Dtos;
using SmartLunch.Database;
using SmartLunch.Database.Entities;

namespace SmartLunch.Services
{
    public class UserCreation(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory
    )
    {
        private IServiceProvider ServiceProvider { get; } = serviceProvider;

        public IHttpClientFactory HttpClientFactory { get; } = httpClientFactory;

        public async Task CreateUserIfNotExistingAsync(ClaimsDto claimsDto)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();

                var client = HttpClientFactory.CreateClient("UserManagement_Api_Client");

                SmartLunchDbContext context =
                    scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();

                UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<
                    UserManager<User>
                >();

                string email = claimsDto.Email!;

                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));
                }

                var getResponse = await client.GetAsync($"api/userManagement/{email}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    if (getResponse.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw getResponse.StatusCode switch
                        {
                            HttpStatusCode.BadRequest => new ArgumentException(
                                "Invalid email format"
                            ),

                            HttpStatusCode.Unauthorized => new UnauthorizedAccessException(
                                "Authentication required"
                            ),

                            _ => new HttpRequestException(
                                $"Request failed: {getResponse.StatusCode}"
                            ),
                        };
                    }
                    else
                    {
                        var userData = new UserCreationDto(
                            0,
                            email,
                            claimsDto.FullName,
                            Guid.NewGuid().ToString(),
                            Guid.NewGuid().ToString(),
                            claimsDto.PhoneNumber,
                            DateTime.Today
                        );

                        var postResponse = await client.PostAsJsonAsync(
                            $"api/userManagement",
                            userData
                        );

                        if (!postResponse.IsSuccessStatusCode)
                        {
                            throw postResponse.StatusCode switch
                            {
                                HttpStatusCode.BadRequest => new ArgumentException(
                                    "Invalid user data provided"
                                ),

                                HttpStatusCode.Conflict => new InvalidOperationException(
                                    "User already exists"
                                ),

                                HttpStatusCode.Unauthorized => new UnauthorizedAccessException(
                                    "Authentication required"
                                ),

                                _ => new HttpRequestException(
                                    $"Request failed: {postResponse.StatusCode}"
                                ),
                            };
                        }
                    }
                }
                else
                {
                    User? user =
                        await getResponse.Content.ReadFromJsonAsync<User>()
                        ?? throw new InvalidOperationException(
                            $"Failed to deserialize user data from response"
                        );

                    await AddRoleToUser(userManager, user);

                    UpdateLoginDate(user);
                }

                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while saving data.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An unexpected error occurred during finding user by email.",
                    ex
                );
            }
        }

        private static async Task AddRoleToUser(UserManager<User> userManager, User user)
        {
            IdentityResult result = await userManager.AddToRoleAsync(user, "Parent");

            var resultValidation = new IdentityResultValidator();
            resultValidation.CheckSuccess(result, "Role assignment failed");
        }

        private static void UpdateLoginDate(User user)
        {
            user.LastLoginDate = DateTime.Today;
        }
    }
}
