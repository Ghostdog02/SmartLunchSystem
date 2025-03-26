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

                SmartLunchDbContext context = scope.ServiceProvider.
                    GetRequiredService<SmartLunchDbContext>();

                UserManager<User> userManager = scope.ServiceProvider.
                    GetRequiredService<UserManager<User>>();

                string email = GetClaim(claims, ClaimTypes.Email, "Email");

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

        private static async Task<User> CreateUserAsync(UserManager<User> userManager, 
            IEnumerable<Claim> claims)
        {
            string firstName = GetClaim(claims, ClaimTypes.GivenName, "First name");
            string email = GetClaim(claims, ClaimTypes.Email, "Email");

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

        private static void UpdateLoginDate(User user)
        {
            var today = DateTime.Now;
            user.LastLoginDate = today;
        }

        private static string GetClaim(IEnumerable<Claim> claims, string claimType, string claimName)
        {
            return claims.FirstOrDefault(claim => claim.Type == claimType).Value
                ?? throw new ArgumentException($"{claimName} claim is missing.");
        }
    }
}
