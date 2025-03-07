using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLunch.Database
{
    public class SeedData
    {
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await SeedRolesAsync(context, userManager);

            if (userManager.FindByEmailAsync("alex.vesely07@gmail.com") == null)
            {
                await SeedAdminUserAsync(context, userManager);
            }
        }

        private async Task SeedAdminUserAsync(SmartLunchDbContext context, UserManager<User> userManager)
        {
            var today = DateTime.Now;

            var user = new User
            {
                UserName = "Ghost_dog",
                Email = "alex.vesely07@gmail.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                LastLoginDate = today,
                RegistrationDate = today
            };

            try
            {
                var creationResult = await userManager.CreateAsync(user);
                if (!creationResult.Succeeded)
                {
                    throw new Exception($"User creation failed: {string.Join(", ", creationResult.Errors.Select(e => e.Description))}");
                }

                var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception($"Adding user to role failed: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }

                await context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while saving data.", ex);
            }

            catch (InvalidOperationException ex)
            {
                throw new Exception("An invalid operation occurred during user processing.", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during user initialization.", ex);
            }
        }

        private async Task SeedRolesAsync(SmartLunchDbContext context, UserManager<User> userManager)
        {

            string[] roles = ["Admin", "Parent", "Cook"];

            try
            {
                foreach (string role in roles)
                {
                    var roleStore = new CustomRoleStore(context);

                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        var identityRole = new IdentityRole<int>(role);
                        var normalizedRoleName = userManager.NormalizeName(role);
                        identityRole.NormalizedName = normalizedRoleName;
                        identityRole.ConcurrencyStamp = Guid.NewGuid().ToString("D");
                        await roleStore.CreateAsync(identityRole);
                    }
                }
            }

            catch (InvalidOperationException ex)
            {

                throw new Exception("An invalid operation occurred during user processing.", ex);
            }

            catch (ArgumentNullException ex)
            {
                throw new Exception("A null parameter has been passed during seeding roles", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during role initialization.", ex);
            }
        }
    }
}
