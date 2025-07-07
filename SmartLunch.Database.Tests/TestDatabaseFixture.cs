using Microsoft.EntityFrameworkCore;

namespace SmartLunch.Database.Tests
{
    public class TestDatabaseFixture
    {
        private const string ConnectionString = @"Server=localhost;Database=SmartLunchSystem;User Id=sa;Password=YourStrong!Passw0rd;Integrated Security=false;TrustServerCertificate=True;MultipleActiveResultSets=true";

        private static readonly Lock _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public static SmartLunchDbContext CreateContext()
            => new SmartLunchDbContext(
                new DbContextOptionsBuilder<SmartLunchDbContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);
    }
}
