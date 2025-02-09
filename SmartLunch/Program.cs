using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Database;

namespace SmartLunch
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://accounts.google.com";
                options.ClientId = builder.Configuration["OpenId:ClientId"];
                options.ClientSecret = builder.Configuration["OpenId:ClientSecret"];
                options.CallbackPath = "/signin-oidc";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.ResponseType = "code";
                options.SkipUnrecognizedRequests = true;
            });

            builder.Services.AddControllersWithViews();
            var connectionString = builder.Configuration.GetConnectionString("SmartLunchContextConnection");
            //builder.Services.AddDbContext<CouponsContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDbContext<SmartLunchDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<User, IdentityRole<int>>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<SmartLunchDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Lockout.AllowedForNewUsers = true;
            });

            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //await ApplyMigrations(app);

            app.Run();
        }

        static async Task ApplyMigrations(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var cancelationTokenSource = new CancellationTokenSource();
            cancelationTokenSource.CancelAfter(TimeSpan.FromMinutes(5));

            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var dbContext = services.GetRequiredService<SmartLunchDbContext>();

            try
            {
                logger.LogInformation("Starting to apply database migrations...");

                // Apply any pending migrations
                await dbContext.Database.MigrateAsync(cancelationTokenSource.Token);

                logger.LogInformation("Database migrations applied successfully.");
            }

            catch (Exception ex)
            {
                // Log the exception if migrations fail
                logger.LogError(ex, "An error occurred while applying database migrations.");

                throw;
            }
        }
    }
}
