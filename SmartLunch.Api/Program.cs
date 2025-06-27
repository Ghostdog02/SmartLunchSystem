using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Database;

namespace SmartLunch.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<SmartLunchDbContext>()
                .AddDefaultTokenProviders();

            var connectionString = builder.Configuration.GetConnectionString("SmartLunchContextConnection");

            builder.Services.AddDbContext<SmartLunchDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            await app.Services.MigrateDbAsync();

            app.Run();
        }
    }
}
