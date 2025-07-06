using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SmartLunch.Database.Tests
{
    public class TestDatabaseFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=SmartLunchSystemUnderTests;MultipleActiveResultSets=true;Integrated Security=true";

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
                        //context.AddRange(
                        //    new Blog { Name = "Blog1", Url = "http://blog1.com" },
                        //    new Blog { Name = "Blog2", Url = "http://blog2.com" });
                        //context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public SmartLunchDbContext CreateContext()
            => new SmartLunchDbContext(
                new DbContextOptionsBuilder<SmartLunchDbContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);
    }
}
