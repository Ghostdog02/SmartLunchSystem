using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLunch.Database.ExtensionClasses
{
    public static class DataExtensions
    {
        public static async Task MigrateDbAsync(this IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SmartLunchDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
