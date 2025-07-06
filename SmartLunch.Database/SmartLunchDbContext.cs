﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Database.Entities;

namespace SmartLunch.Database
{
    public class SmartLunchDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public SmartLunchDbContext(DbContextOptions options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
            });

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

    public class CustomRoleStore : RoleStore<IdentityRole<int>, SmartLunchDbContext, int>
    {
        public CustomRoleStore(SmartLunchDbContext context)
            : base(context) { }
    }
}
