using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace SmartLunch.Database
{
    public class SeedData
    {
        public async void InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = ["Admin", "Parent", "Cook"];

            foreach (string role in roles)
            {
                var roleStore = new CustomRoleStore(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    var identityRole = new IdentityRole<int>(role);
                    var normalizedRoleName = userManager.NormalizeName(role);
                    identityRole.NormalizedName = normalizedRoleName;
                    //identityRole.NormalizedName = role.Normalize();
                    identityRole.ConcurrencyStamp = Guid.NewGuid().ToString("D");
                    await roleStore.CreateAsync(identityRole);
                }
            }

            //var users = new List<User>();
            //string password = "oXMR4TgdfQAEqEN";

            //var user = new User
            //{
            //    UserName = "Admin",
            //    Email = "admin@admin.com",
            //    PhoneNumber = "0882452245",
            //    EmailConfirmed = true,
            //    PhoneNumberConfirmed = true,
            //    LockoutEnabled = false,
            //};

            //var user2 = new User
            //{
            //    UserName = "Parent",
            //    Email = "parent@parent.com",
            //    PhoneNumber = "0882432245",
            //    EmailConfirmed = true,
            //    PhoneNumberConfirmed = true,
            //    LockoutEnabled = false,
            //};

            //users.Add(user);
            //users.Add(user2);

            //foreach (var normalUser in users)
            //{
            //    if (await userManager.FindByEmailAsync(normalUser.Email) == null)
            //    {
            //        await userManager.CreateAsync(normalUser, password);

            //        //Change role Parameter if you want to change you username later
            //        await userManager.AddToRoleAsync(normalUser, normalUser.UserName);
            //    }
            //}

            await context.SaveChangesAsync();
        }
    }
}
