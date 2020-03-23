using System.Linq;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ogma3.Data.Models;
using Ogma3.Services.Mailer;
using Utils;

namespace Ogma3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        public IConfiguration Configuration { get; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
                Configuration.GetConnectionString("PostgresConnection"))
            );

            // Identity
            services.AddIdentity<User, IdentityRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                    config.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<OgmaUserManager>()
                .AddDefaultTokenProviders();
            
            // Claims
            services.AddScoped<IUserClaimsPrincipalFactory<User>, OgmaClaimsPrincipalFactory>();
            
            // Argon2 hasher
            services.UpgradePasswordSecurity().UseArgon2<User>();

            // Email
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            
            // Backblaze
            var b2Options = Configuration.GetSection("B2").Get<B2Options>();
            var b2Client = new B2Client(B2Client.Authorize(b2Options));
            services.AddSingleton<IB2Client>(b2Client);

            // Auth
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/login";
                });
            
            // Auth
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });
            
            // Compression
            services.AddResponseCompression();
            
            // Runtime compilation
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            services.AddRazorPages().AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Admin", "/", "RequireAdminRole");
                });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OgmaUserManager userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext ctx)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
            
            // Compression
            app.UseResponseCompression();

            // Seed data
            SeedRoles(roleManager);
            SeedUserRoles(userManager);
            SeedRatings(ctx);
            SeedIcons(ctx);

        }


        // Seeding

        private static void SeedRoles (RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.RoleExistsAsync("Admin").Result) return;
            
            var role = new IdentityRole { Name = "Admin" };
            var roleResult = roleManager.CreateAsync(role).Result;
        }

        private static void SeedUserRoles(OgmaUserManager userManager)
        {
            var user = userManager.FindByNameAsync("Angius").Result;
            if (user != null)
            {
                userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }

        private static void SeedRatings(ApplicationDbContext ctx)
        {
            Rating[] ratings =
            {
                new Rating {Name = "Everyone", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"},
                new Rating {Name = "Teen", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"},
                new Rating {Name = "Mature", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"},
                new Rating {Name = "Adult", Description = "12345", Icon = Lorem.Picsum(100), IconId = "12345"}
            };

            foreach (var rating in ratings)
            {
                if (ctx.Ratings.FirstOrDefault(r => r.Name == rating.Name) == null)
                {
                    ctx.Ratings.Add(rating);
                }
                ctx.SaveChanges();
            }
        }

        private static void SeedIcons(ApplicationDbContext ctx)
        {
            string[] icons =
            {
                "book",
                "bookmark_border",
                "check_circle",
                "delete",
                "eco",
                "explore",
                "extension",
                "face",
                "favorite_border",
                "fingerprint",
                "star_border",
                "report_problem",
                "thumb_up",
                "thumb_down",
                "visibility",
                "new_releases",
                "outlined_flag",
                "toys",
                "palette",
                "casino",
                "spa"
            };
            foreach (var i in icons)
            {
                if (ctx.Icons.FirstOrDefault(ico => ico.Name == i) == null)
                {
                    ctx.Icons.Add(new Icon {Name = i});
                }
            }

            ctx.SaveChanges();
        }
    }
}
