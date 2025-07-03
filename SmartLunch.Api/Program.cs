using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SmartLunch.Database;
using SmartLunch.Database.Entities;
using SmartLunch.Database.ExtensionClasses;

namespace SmartLunch.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString(
                "SmartLunchConnection"
            );

            builder.Services.AddDbContext<SmartLunchDbContext>(
                options =>
                    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()),
                ServiceLifetime.Scoped
            );

            builder
                .Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            builder
                .Services.AddIdentity<User, IdentityRole<int>>(options =>
                    options.SignIn.RequireConfirmedAccount = true
                )
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<SmartLunchDbContext>()
                .AddDefaultTokenProviders();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "My API",
                        Version = "v1",
                        Description = "Interactive API docs",
                    }
                );

                // (Optional) Include XML comments for richer docs:
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Serves /swagger/v1/swagger.json
                app.UseSwagger();

                // Hosts Swagger UI at /swagger
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = string.Empty; // Serve UI at root (optional)
                });

                //app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            var seeder = new SeedData(app.Services);

            await app.Services.MigrateDbAsync();
            await seeder.InitializeAsync();

            app.Run();
        }
    }
}
