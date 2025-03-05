using Microsoft.AspNetCore.Identity;
using SmartLunch.Database;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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

                User currentUser = FindUserByEmailAsync(userManager, claims).Result;

                if (currentUser == null)
                {
                    currentUser = CreateUserAsync(userManager, claims).Result;
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

        private async Task<User> FindUserByEmailAsync(UserManager<User> userManager,
            IEnumerable<Claim> claims)
        {
            try
            {
                string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                User? user = await userManager.FindByEmailAsync(email);
                return user;
            }

            catch (ArgumentNullException ex)
            {

                throw new Exception("Null reference has been passed during finding user by email", ex);
            }

            catch (InvalidOperationException ex)
            {

                throw new Exception("An invalid operation occurred during finding user by email.", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during finding user by email.", ex);
            }


        }

        private async Task<User> CreateUserAsync(UserManager<User> userManager, IEnumerable<Claim> claims)
        {
            try
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
                    // Handle errors (e.g., log or throw an exception)
                    throw new Exception($"User creation failed: {string.Join(", ", creationResult.Errors)}");
                }

                return user;
            }

            catch (ArgumentException ex)
            {

                throw new Exception("An argument exception occurred during user creation.", ex);
            }

            catch (InvalidOperationException ex)
            {
                throw new Exception("An invalid operation occurred during user creation.", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during user creation.", ex);
            }

        }

        private async Task AddRoleToUser(UserManager<User> userManager, User user)
        {
            try
            {
                await userManager.AddToRoleAsync(user, "Parent");
            }

            catch (ArgumentException ex)
            {

                throw new Exception("An argument exception occurred during role assigning.", ex);
            }

            catch (InvalidOperationException ex)
            {
                throw new Exception("An invalid operation occurred during role assigning.", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during role assigning.", ex);
            }

        }

        public void UpdateLoginDate(User user)
        {
            var today = DateTime.Now;
            user.LastLoginDate = today;
        }
    }
}
