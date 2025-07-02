using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SmartLunch.Database
{
    public class SmartLunchDbContextFactory : IDesignTimeDbContextFactory<SmartLunchDbContext>
    {
        // public SmartLunchDbContext CreateDbContext(string[] args)
        // {
        //     var optionsBuilder = new DbContextOptionsBuilder<SmartLunchDbContext>();

        //     // either hard-code your conn-string:
        //     optionsBuilder.UseSqlServer(
        //         "Server=localhost;Database=SmartLunchSystem;User Id=sa;Password=YourStrong!Passw0rd;Integrated Security=false;TrustServerCertificate=True;"
        //     );

        //     // or pull from a JSON file / env-var:
        //     //var config = new ConfigurationBuilder()…;
        //     //optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));

        //     return new SmartLunchDbContext(optionsBuilder.Options);
        // }

        public SmartLunchDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartLunchDbContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("SmartLunchConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new SmartLunchDbContext(optionsBuilder.Options);
        }
    }
}
