using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using SmartLunch.Database.ExtensionClasses;

namespace SmartLunch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient(
                "UserManagementAPI",
                client =>
                {
                    client.BaseAddress = new Uri("http://localhost:5116");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                }
            );

            builder.Services.AddHttpClient(
                "RoleManagementAPI",
                client =>
                {
                    client.BaseAddress = new Uri("http://localhost:5116");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                }
            );

            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(
                    GoogleDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
                        options.ClientSecret = builder.Configuration[
                            "Authentication:Google:ClientSecret"
                        ]!;
                        options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                        options.Scope.Add("profile");
                        options.Scope.Add("email");
                        //options.CallbackPath = "/signin-google";
                    }
                );

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
                builder.Services.AddHttpsRedirection(options =>
                {
                    options.HttpsPort = 7235;
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                });
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
