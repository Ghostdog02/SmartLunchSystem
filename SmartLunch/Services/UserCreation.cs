using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Database;
using System.Security.Claims;

namespace SmartLunch.Services
{
    public class UsersCreation
    {
        private IServiceProvider ServiceProvider { get; }

        public UsersCreation(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task CreateUserIfNotExistingAsync(IEnumerable<Claim> claims)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                SmartLunchDbContext context = scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();
                UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value
                    ?? throw new ArgumentException("Email claim is missing.");

                User? currentUser = await userManager.FindByEmailAsync(email);

                if (currentUser == null)
                {
                    currentUser = await CreateUserAsync(userManager, claims);
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

        private async Task<User> CreateUserAsync(UserManager<User> userManager, IEnumerable<Claim> claims)
        {
            string? firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = new User
            {
                UserName = firstName,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = false,
                RegistrationDate = DateTime.Now
            };

            var creationResult = await userManager.CreateAsync(user);
            if (!creationResult.Succeeded)
            {
                var errorMessages = string.Join(", ", creationResult.Errors.
                    Select(e => e.Description));

                throw new Exception($"User creation failed: {errorMessages}");
            }

            return user;
        }

        private async Task AddRoleToUser(UserManager<User> userManager, User user)
        {
            IdentityResult result = await userManager.AddToRoleAsync(user, "Parent");

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.
                    Select(e => e.Description));

                throw new Exception($"Role assignment failed: {string.Join(", ", result.Errors)}");
            }

        }

        private void UpdateLoginDate(User user)
        {
            var today = DateTime.Now;
            user.LastLoginDate = today;
        }
    }
}
