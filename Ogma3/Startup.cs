using System;
using System.Reflection;
using System.Text.Json.Serialization;
using B2Net;
using B2Net.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
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
using Npgsql;
using NpgSqlGenerators;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Notifications;
using Ogma3.Data.Roles;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Filters;
using Ogma3.Infrastructure.Formatters;
using Ogma3.Infrastructure.MediatR.Behaviours;
using Ogma3.Infrastructure.Middleware;
using Ogma3.Infrastructure.NSwag.OperationProcessors;
using Ogma3.Infrastructure.StartupGenerators;
using Ogma3.Services;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.FileUploader;
using Ogma3.Services.Initializers;
using Ogma3.Services.Mailer;
using Ogma3.Services.UserService;
using reCAPTCHA.AspNetCore;
using Serilog;
using static Ogma3.Services.RoutingHelpers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Ogma3;

public class Startup
{
	// ReSharper disable once UnusedParameter.Local
	public Startup(IConfiguration configuration, IWebHostEnvironment env)
	{
		Configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
			.AddEnvironmentVariables()
			// WARN: It probably should not be used in prod, switch to DI instead
			.AddUserSecrets(Assembly.GetAssembly(GetType()) ?? throw new NullReferenceException("The assembly was, somehow, null"))
			.Build();
		// Configuration = configuration;
	}

	private IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		// Profiler
		services.AddMiniProfiler().AddEntityFramework();

		// Database
		var conn = Environment.GetEnvironmentVariable("DATABASE_URL") ?? Configuration.GetConnectionString("DbConnection");
		var npgSourceBuilder = new NpgsqlDataSourceBuilder(conn);
		var source = npgSourceBuilder.MapPostgresEnums().Build();
		services
			.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(source))
			.AddDatabaseDeveloperPageExceptionFilter();

		// Repositories
		services
			.AddScoped<UserRepository>()
			.AddScoped<ClubRepository>()
			.AddScoped<StoriesRepository>()
			.AddScoped<NotificationsRepository>();

		// Middleware
		services
			.AddTransient<RequestTimestampMiddleware>()
			.AddTransient<UserBanMiddleware>();

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
			.AddUserStore<UserStore<
				OgmaUser,
				OgmaRole,
				ApplicationDbContext,
				long,
				IdentityUserClaim<long>,
				UserRole,
				IdentityUserLogin<long>,
				IdentityUserToken<long>,
				IdentityRoleClaim<long>>>()
			.AddRoleStore<RoleStore<OgmaRole, ApplicationDbContext, long, UserRole, IdentityRoleClaim<long>>>();

		// Add services
		services
			.AddScoped<IUserService, UserService>()
			.AddSingleton<ICodeGenerator, CodeGenerator>();

		// Claims
		services.AddScoped<IUserClaimsPrincipalFactory<OgmaUser>, OgmaClaimsPrincipalFactory>();
		services.AddScoped(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User);

		// Argon2 hasher
		services
			.UpgradePasswordSecurity()
			.UseArgon2<OgmaUser>();

		// HttpClient factory
		services.AddHttpClient();

		// Email
		services
			.AddTransient<IEmailSender, PostmarkMailer>()
			.Configure<PostmarkOptions>(Configuration);

		// Backblaze
		var b2Options = Configuration.GetSection("B2").Get<B2Options>();
		services.AddSingleton<IB2Client>(new B2Client(b2Options));

		// File uploader
		services.AddSingleton<ImageUploader>();

		// ReCaptcha
		services
			.AddTransient<IRecaptchaService, RecaptchaService>()
			.Configure<RecaptchaSettings>(Configuration.GetSection("RecaptchaSettings"));

		// Seeding
		services.AddAsyncInitializer<DbSeedInitializer>();

		// Auth
		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

		// Auth
		services.AddAuthorization(options => { options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")); });


		// Cookies
		services.ConfigureApplicationCookie(options =>
		{
			options.LoginPath = new PathString("/login");
			options.LogoutPath = new PathString("/logout");
			options.AccessDeniedPath = new PathString("/login");
			options.Cookie.SameSite = SameSiteMode.Lax;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

			options.Events.OnRedirectToLogin = HandleApiRequest(StatusCodes.Status401Unauthorized, options.Events.OnRedirectToLogin);
			options.Events.OnRedirectToAccessDenied = HandleApiRequest(StatusCodes.Status403Forbidden, options.Events.OnRedirectToLogin);
		});

		// Compression
		services.AddResponseCompression();

		// Cache
		services.AddMemoryCache();

		// Automapper
		services.AddAutoMapper(typeof(Startup));

		// Runtime compilation
		services
			.AddControllersWithViews()
			.AddRazorRuntimeCompilation();

		// Razor
		services
			.AddRazorPages()
			.AddRazorPagesOptions(options =>
			{
				options.Conventions.AuthorizeAreaFolder("Admin", "/", "RequireAdminRole");
			})
			.AddRazorRuntimeCompilation();

		// MVC
		services
			.AddMvc(options =>
			{
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
				options.Filters.Add<ValidationExceptionFilter>();
			})
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			});

		// Fluent Validation
		services
			.AddFluentValidationAutoValidation()
			.AddValidatorsFromAssemblyContaining<Startup>()
			.AddFluentValidationClientsideAdapters(cfg =>
			{
				cfg.ClientValidatorFactories[typeof(IFileSizeValidator)] = (_, rule, component) =>
					new FileSizeClientValidator(rule, component);
			});

		// MediatR
		services
			.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly))
			.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

		// Custom formatters
		services.AddControllers(options => { options.OutputFormatters.Insert(0, new RssOutputFormatter(Configuration)); });

		// OpenAPI
		services.AddOpenApiDocument(settings =>
		{
			settings.DocumentName = "public";
			settings.OperationProcessors.Insert(0, new ExcludeRssProcessor());
			settings.OperationProcessors.Insert(1, new ExcludeInternalApisProcessor());
			settings.SchemaSettings.SchemaNameGenerator = new NSwagNestedNameGenerator();
		});
		services.AddOpenApiDocument(settings =>
		{
			settings.DocumentName = "internal";
			settings.OperationProcessors.Insert(0, new ExcludeRssProcessor());
			settings.OperationProcessors.Insert(1, new IncludeInternalApisProcessor());
			settings.SchemaSettings.SchemaNameGenerator = new NSwagNestedNameGenerator();
		});
	}


	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
		app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"),
			appBuilder => { appBuilder.UseStatusCodePagesWithReExecute("/api/error?code={0}"); });
		app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"),
			appBuilder => { appBuilder.UseStatusCodePagesWithReExecute("/StatusCode/{0}"); });

		// Redirects
		app.UseHttpsRedirection();
		app.UseRedirectMiddleware(options =>
		{
			options.Redirects.Add("/.well-known/change-password", "/identity/account/manage/changepassword");
		});

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
				if (context.File.Name.Contains("service-worker"))
				{
					Log.Information("Serving a service worker");
					context.Context.Response.Headers.Append("Service-Worker-Allowed", "/");
				}
			},
			ContentTypeProvider = extensionsProvider
		});
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseBanMiddleware();

		// OpenAPI
		app.UseOpenApi();
		app.UseSwaggerUi3(config =>
		{
			config.TransformToExternalPath = (s, _) => s;
			config.CustomStylesheetPath = "https://cdn.genfic.net/file/Ogma-net/swagger-dark.css";
		});

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapRazorPages();
			endpoints.MapControllers();
		});

		// Compression
		app.UseResponseCompression();

		// Generate JS manifest
		new JavascriptFilesManifestGenerator(env).Generate("js/dist", "js/bundle");
	}
}