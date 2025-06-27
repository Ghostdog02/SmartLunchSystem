using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Api.Dtos;
using SmartLunch.Database;

namespace SmartLunch.Services
{
    public class UserCreation
    {
        private IServiceProvider ServiceProvider { get; }

        public UserCreation(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task CreateUserIfNotExistingAsync(ClaimsDto claimsDto)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();

                SmartLunchDbContext context = scope.ServiceProvider.
                    GetRequiredService<SmartLunchDbContext>();

                UserManager<User> userManager = scope.ServiceProvider.
                    GetRequiredService<UserManager<User>>();

                string email = claimsDto.Email!;

                User? currentUser = await userManager.FindByEmailAsync(email);

                if (currentUser == null)
                {
                    currentUser = await CreateUserAsync(userManager, claimsDto);
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

        private static void UpdateLoginDate(User user)
        {
            var today = DateTime.Now;
            user.LastLoginDate = today;
        }
    }
}
