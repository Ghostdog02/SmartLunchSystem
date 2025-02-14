using Microsoft.AspNetCore.Identity;
using SmartLunch.Database;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLunch.Services
{
    public class CreationOfUsers
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        private IServiceProvider ServiceProvider { get; }

        public CreationOfUsers(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            ServiceProvider = serviceProvider;
            HttpContextAccessor = httpContextAccessor;
        }

        public async Task CreateUsersAsync(IEnumerable<Claim> claims)
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var currentUser = await userManager.GetUserAsync(HttpContextAccessor.HttpContext.User);
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (await userManager.FindByEmailAsync(currentUser.Email) == null)
            {
                await userManager.CreateAsync(currentUser, currentUser.PasswordHash);
                if (currentUser.UserName == "Ghost_dog 07")
                {
                    await userManager.AddToRoleAsync(currentUser, "Admin");
                }

                else
                {
                    await userManager.AddToRoleAsync(currentUser, "Parent");
                }
                
            }

            await context.SaveChangesAsync();
        }
    }
} 
