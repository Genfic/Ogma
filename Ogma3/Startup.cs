using System;
using System.Linq;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ogma3.Data.Models;
using Ogma3.Services;
using Ogma3.Services.Initializers;
using Ogma3.Services.Mailer;
using reCAPTCHA.AspNetCore;
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
                Configuration.GetConnectionString("DbConnection")
             ));
            
            // Routing
            services.AddRouting(options => options.LowercaseUrls = true);
            
            // HttpContextAccessor
            services.AddHttpContextAccessor();

            // Identity
            services.AddIdentity<OgmaUser, Role>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                    config.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<OgmaUserManager>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<OgmaUser, Role, ApplicationDbContext, long>>()
                .AddRoleStore<RoleStore<Role, ApplicationDbContext, long>>();
            
            // Claims
            services.AddScoped<IUserClaimsPrincipalFactory<OgmaUser>, OgmaClaimsPrincipalFactory>();
            
            // Argon2 hasher
            services.UpgradePasswordSecurity().UseArgon2<OgmaUser>();

            // Email
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            
            // Backblaze
            var b2Options = Configuration.GetSection("B2").Get<B2Options>();
            var b2Client = new B2Client(B2Client.Authorize(b2Options));
            services.AddSingleton<IB2Client>(b2Client);
            
            // File uploader
            services.AddSingleton<FileUploader>();
            
            // ReCaptcha
            services.Configure<RecaptchaSettings>(Configuration.GetSection("RecaptchaSettings"));
            services.AddTransient<IRecaptchaService, RecaptchaService>();
            
            // Seeding
            services.AddAsyncInitializer<DbSeedInitializer>();

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OgmaUserManager userManager, RoleManager<Role> roleManager, ApplicationDbContext ctx)
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
        }
    }
}
