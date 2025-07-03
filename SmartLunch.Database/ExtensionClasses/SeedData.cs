using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartLunch.Database.Entities;

namespace SmartLunch.Database.ExtensionClasses
{
    public class SeedData
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SmartLunchDbContext _context;
        private readonly UserManager<User> _userManager;

        public SeedData(IServiceProvider serviceProvider)
        {
            _serviceProvider =
                serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            using var scope = _serviceProvider.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        }

        public async Task InitializeAsync()
        {
            try
            {
                await SeedRolesAsync(_context);
                await SeedAdminUserAsync(_context, _userManager);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while saving data.", ex);
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
                throw new Exception("An unexpected error occurred during user initialization.", ex);
            }
        }

        private async Task SeedAdminUserAsync(
            SmartLunchDbContext context,
            UserManager<User> userManager
        )
        {
            var adminEmail = "alex.vesely07@gmail.com";
            if (userManager.FindByEmailAsync(adminEmail) != null)
            {
                return;
            }

            var today = DateTime.Today;

            var user = new User
            {
                UserName = "Ghost_dog",
                Email = adminEmail,
                EmailConfirmed = true,
                LockoutEnabled = false,
                LastLoginDate = today,
                RegistrationDate = today,
            };

            var creationResult = await userManager.CreateAsync(user);

            var userCreationResultValidator = new IdentityResultValidator();
            userCreationResultValidator.CheckSuccess(creationResult, "User creation failed");

            var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");

            var roleResultValidator = new IdentityResultValidator();
            roleResultValidator.CheckSuccess(addToRoleResult, "Adding user to role failed");

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(SmartLunchDbContext context)
        {
            string[] roles = ["Admin", "NormalUser", "SuperUser"];

            foreach (string role in roles)
            {
                var roleStore = new CustomRoleStore(context);

                if (await context.Roles.FindAsync(role) == null)
                {
                    var identityRole = new IdentityRole<int>(role)
                    {
                        NormalizedName = role.ToUpperInvariant(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                    };

                    await roleStore.CreateAsync(identityRole);
                }
            }

            await context.SaveChangesAsync();
        }
    }

    public class IdentityResultValidator
    {
        public void CheckSuccess(IdentityResult result, string description)
        {
            if (!result.Succeeded)
            {
                string errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));

                throw new Exception($"{description}: {string.Join(", ", result.Errors)}");
            }
        }
    }
}
