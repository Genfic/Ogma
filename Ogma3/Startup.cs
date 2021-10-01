using System;
using System.Reflection;
using System.Text.Json.Serialization;
using B2Net;
using B2Net.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using LinqToDB.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Notifications;
using Ogma3.Data.Roles;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Formatters;
using Ogma3.Infrastructure.MediatR.Behaviours;
using Ogma3.Services;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.FileUploader;
using Ogma3.Services.Initializers;
using Ogma3.Services.Mailer;
using Ogma3.Services.Middleware;
using Ogma3.Services.RssService;
using Ogma3.Services.UserService;
using reCAPTCHA.AspNetCore;
using static Ogma3.Services.RoutingHelpers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Ogma3;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
            .AddEnvironmentVariables()
            // WARN: It probably should not be used in prod, switch to DI instead
            .AddUserSecrets(Assembly.GetAssembly(GetType()))
            .Build();
        // Configuration = configuration;
    }
        
    public IConfiguration Configuration { get; }
        
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Profiler
        services.AddMiniProfiler().AddEntityFramework();
        
        // Database
        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL") ?? Configuration.GetConnectionString("DbConnection")));
        services.AddDatabaseDeveloperPageExceptionFilter();
            
        // Repositories
        services.AddScoped<UserRepository>();
        services.AddScoped<ClubRepository>();
        services.AddScoped<StoriesRepository>();
        services.AddScoped<NotificationsRepository>();
            
        // Validators
        services.AddValidatorsFromAssemblyContaining<Startup>();
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        // Custom persistent config
        services.AddSingleton(OgmaConfig.Init("config.json"));
            
        // Comment redirector
        services.AddScoped<CommentRedirector>();

        // Routing
        services.AddRouting(options => options.LowercaseUrls = true);
            
        // HttpContextAccessor
        services.AddHttpContextAccessor();
            
        // ActionContextAccessor
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
        // UrlHelperFactory
        services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

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
        services.AddScoped<IUserService, UserService>();
        // RSS service
        services.AddTransient<IRssService, RssService>();
        // Code generator service
        services.AddSingleton<ICodeGenerator, CodeGenerator>();
            
        // Claims
        services.AddScoped<IUserClaimsPrincipalFactory<OgmaUser>, OgmaClaimsPrincipalFactory>();
        services.AddScoped(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User);
            
        // Argon2 hasher
        services.UpgradePasswordSecurity().UseArgon2<OgmaUser>();

        // HttpClient factory
        services.AddHttpClient();
            
        // Email
        services.AddTransient<IEmailSender, MailGunMailer>();
        services.Configure<MailGunOptions>(Configuration);
            
        // Backblaze
        var b2Options = Configuration.GetSection("B2").Get<B2Options>();
        var b2Client = new B2Client(B2Client.Authorize(b2Options));
        services.AddSingleton<IB2Client>(b2Client);
            
        // File uploader
        services.AddSingleton<ImageUploader>();
            
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
        services.AddAutoMapper(typeof(Startup));
            
        // Runtime compilation
        services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
        // Razor
        services.AddRazorPages().AddRazorPagesOptions(options =>
        {
            options.Conventions.AuthorizeAreaFolder("Admin", "/", "RequireAdminRole");
            options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/ChangePassword", "/.well-known/change-password");
        });
            
        // MVC
        services
            .AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
                options.ConfigureClientsideValidation(clientside =>
                {
                    clientside.ClientValidatorFactories[typeof(IFileSizeValidator)] = (_, rule, component) =>
                        new FileSizeClientValidator(rule, component);
                });
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            
        // MediatR
        services.AddMediatR(typeof(Startup));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            
        // Custom formatters
        services.AddControllers(options =>
        {
            options.OutputFormatters.Insert(0, new RssOutputFormatter(Configuration));
        });
            
        // OpenAPI
        services.AddOpenApiDocument(settings =>
        {
            settings.SchemaNameGenerator = new NSwagNestedNameGenerator();
        });
            
        // Linq2DB extension
        LinqToDBForEFTools.Initialize();
    }



    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OgmaUserManager userManager, RoleManager<OgmaRole> roleManager)
    {
        // Profiler
        app.UseMiniProfiler();
            
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
            appBuilder.UseStatusCodePagesWithReExecute("/api/error?code={0}");
        });
        app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
        {
            appBuilder.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
        });

        app.UseHttpsRedirection();
            
        // Map file extensions
        var extensionsProvider = new FileExtensionContentTypeProvider();
        extensionsProvider.Mappings.Add(".avif", "image/avif");

        // Serve static files with cache headers and compression
        app.UseStaticFiles(new StaticFileOptions
        {
            HttpsCompression = HttpsCompressionMode.Compress,
            OnPrepareResponse = context =>
            {
                context.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(365)
                };
            },
            ContentTypeProvider = extensionsProvider
        });

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseBanMiddleware();
            
        // Swagger
        app.UseOpenApi();
        app.UseSwaggerUi3();
            
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
            
        // Compression
        app.UseResponseCompression();
    }
}