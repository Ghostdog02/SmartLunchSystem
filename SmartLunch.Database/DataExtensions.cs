using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SmartLunch.Database
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
