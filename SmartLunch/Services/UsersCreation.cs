using Microsoft.AspNetCore.Identity;
using SmartLunch.Database;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLunch.Services
{
    public class UsersCreation
    {
        private IServiceProvider ServiceProvider { get; }

        public UsersCreation(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task CreateUsersAsync(IEnumerable<Claim> claims)
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            User? currentUser = await userManager.FindByEmailAsync(email);

            if (currentUser == null)
            {
                var today = DateTime.Now;
                string? firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

                var user = new User
                {
                    UserName = firstName,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    LastLoginDate = today,
                    RegistrationDate = today
                };

                var creationResult = await userManager.CreateAsync(user);
                if (!creationResult.Succeeded)
                {
                    // Handle errors (e.g., log or throw an exception)
                    throw new Exception($"User creation failed: {string.Join(", ", creationResult.Errors)}");
                }

                if (user.UserName == "Ghost_dog")
                {
                    var roleAdditionResult = await userManager.AddToRoleAsync(user, "Admin");
                }

                else
                {
                    await userManager.AddToRoleAsync(user, "Parent");
                }

            }

            await context.SaveChangesAsync();
        }
    }
} 
