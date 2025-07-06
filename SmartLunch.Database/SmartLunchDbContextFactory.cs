using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartLunch.Database
{
    public class SmartLunchDbContextFactory : IDesignTimeDbContextFactory<SmartLunchDbContext>
    {
        public SmartLunchDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartLunchDbContext>();

            var basePath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(),
                            "..",
                            "SmartLunch.Api"
                )
            );

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("SmartLunchConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new SmartLunchDbContext(optionsBuilder.Options);
        }
    }
}
