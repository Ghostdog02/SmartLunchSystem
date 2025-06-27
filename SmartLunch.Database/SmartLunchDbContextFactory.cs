using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartLunch.Database
{
    public class SmartLunchDbContextFactory
        : IDesignTimeDbContextFactory<SmartLunchDbContext>
    {
        public SmartLunchDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartLunchDbContext>();

            // either hard-code your conn-string:
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=SmartLunchSystem;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
            );

            // or pull from a JSON file / env-var:
            //var config = new ConfigurationBuilder()…;
            //optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));

            return new SmartLunchDbContext(optionsBuilder.Options);
        }
    }
}
