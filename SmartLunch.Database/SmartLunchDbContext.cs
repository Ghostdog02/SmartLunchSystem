using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartLunch.Database
{
    public class SmartLunchDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        //DbSet<User> Users { get; set; }

        //DbSet<IdentityRole<int>> Roles { get; set; }

        //DbSet<IdentityUserRole<int>> UserRoles { get; set; }

        //DbSet<IdentityUserClaim<int>> UserClaims { get; set; }

        //DbSet<IdentityUserLogin<int>> UserLogins { get; set; }

        //DbSet<IdentityRoleClaim<int>> RoleClaims { get; set; }

        //DbSet<IdentityUserToken<int>> UserTokens { get; set; }

        public SmartLunchDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<IdentityRole<int>>(entity =>
            {
                entity.ToTable(name: "Roles");
            });

            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }
    }
}
