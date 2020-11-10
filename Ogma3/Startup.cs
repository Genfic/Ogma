using System;
using System.Text.Json.Serialization;
using AutoMapper;
using B2Net;
using B2Net.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Services.FileUploader;
using Ogma3.Services.Initializers;
using Ogma3.Services.Mailer;
using Ogma3.Services.Middleware;
using Ogma3.Services.UserService;
using reCAPTCHA.AspNetCore;
using static Ogma3.Services.RoutingHelpers;

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
                Environment.GetEnvironmentVariable("DATABASE_URL") ?? Configuration.GetConnectionString("DbConnection")
             ));
            services.AddDatabaseDeveloperPageExceptionFilter();
            
            // Repositories
            services.AddScoped<UserRepository>();
            services.AddScoped<ClubRepository>();
            services.AddScoped<ThreadRepository>();
            services.AddScoped<StoriesRepository>();
            services.AddScoped<TagsRepository>();
            services.AddScoped<ChaptersRepository>();
            services.AddScoped<BlogpostsRepository>();
            services.AddScoped<CommentsRepository>();
            services.AddScoped<FoldersRepository>();

            // Custom persistent config
            services.AddSingleton(OgmaConfig.Init("config.json"));

            // Routing
            services.AddRouting(options => options.LowercaseUrls = true);
            
            // HttpContextAccessor
            services.AddHttpContextAccessor();

            // Identity
            services.AddIdentity<OgmaUser, OgmaRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                    config.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_ ";
                    config.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<OgmaUserManager>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<OgmaUser, OgmaRole, ApplicationDbContext, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityUserToken<long>, IdentityRoleClaim<long>>>()
                .AddRoleStore<RoleStore<OgmaRole, ApplicationDbContext, long, UserRole, IdentityRoleClaim<long>>>();
            
            // Logged in user service
            services.AddTransient<IUserService, UserService>();
            
            // Claims
            services.AddScoped<IUserClaimsPrincipalFactory<OgmaUser>, OgmaClaimsPrincipalFactory>();
            services.AddScoped(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User);
            
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            // Auth
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });

            
            // Cookies
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.AccessDeniedPath = new PathString("/login");
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                options.Events.OnRedirectToLogin = 
                    HandleApiRequest(StatusCodes.Status401Unauthorized, options.Events.OnRedirectToLogin);
                options.Events.OnRedirectToAccessDenied =
                    HandleApiRequest(StatusCodes.Status403Forbidden, options.Events.OnRedirectToLogin);
            });
            
            // Compression
            services.AddResponseCompression();
            
            // Cache
            services.AddMemoryCache();
            
            // Automapper
            services.AddAutoMapper(typeof(AutoMapperProfile));
            
            // Runtime compilation
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            // Razor
            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Admin", "/", "RequireAdminRole");
                options.Conventions.AddPageRoute("/Club/Index", "/club/{slug}-{id}");
            });
            
            // MVC
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OgmaUserManager userManager, RoleManager<OgmaRole> roleManager, ApplicationDbContext ctx)
        {
            // Request timestamp
            app.UseRequestTimestamp();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            // Handle errors
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            {
                appBuilder.UseStatusCodePagesWithRedirects("/api/error?code={0}");
            });
            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            {
                appBuilder.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
            });

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
            
            // Apply migrations
            if (!env.IsDevelopment())
            {
                ctx.Database.Migrate();
            }
        }
        
        
    }
}
