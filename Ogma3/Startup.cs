using System.IO.Compression;
using System.Text.Json.Serialization;
using B2Net;
using B2Net.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Net.Http.Headers;
using NpgSqlGenerators;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Notifications;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Compression;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Filters;
using Ogma3.Infrastructure.Middleware;
using Ogma3.Infrastructure.OpenApi;
using Ogma3.Infrastructure.OpenApi.Transformers;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.ServiceDefaults;
using Ogma3.Services;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.ETagService;
using Ogma3.Services.FileUploader;
using Ogma3.Services.Initializers;
using Ogma3.Services.Mailer;
using Ogma3.Services.TurnstileService;
using Ogma3.Services.UserService;
using Scalar.AspNetCore;
using Serilog;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.CysharpMemoryPack;
using static Ogma3.Services.RoutingHelpers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Ogma3;

public static class Startup
{
	// This method gets called by the runtime. Use this method to add services to the container.
	public static TBuilder ConfigureServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	{
		var services = builder.Services;
		var configuration = builder.Configuration;

		// Profiler
		services.AddMiniProfiler().AddEntityFramework();

		// Compression
		services.AddResponseCompression(options => {
				options.EnableForHttps = true;
				options.Providers.Add<ZstdCompressionProvider>();
				options.Providers.Add<BrotliCompressionProvider>();
				options.Providers.Add<GzipCompressionProvider>();
			})
			.Configure<ZstdCompressionProvider.Options>(o => o.CompressionLevel = CompressionLevel.Optimal);

		// Database
		var conn = configuration.GetConnectionString("ogma3-db") ?? configuration.GetConnectionString("DbConnection");
		services
			.AddDbContext<ApplicationDbContext>(options => options
				.UseNpgsql(conn, o => o.MapPostgresEnums())
			)
			.AddDatabaseDeveloperPageExceptionFilter();

		// Repositories
		services
			.AddScoped<UserRepository>()
			.AddScoped<ClubRepository>()
			.AddScoped<NotificationsRepository>();

		// Middleware
		services
			.AddTransient<RequestTimestampMiddleware>()
			.AddTransient<UserBanMiddleware>();
		builder.UseAddHeaders();

		// Validators
		services.AddValidatorsFromAssemblyContaining<Program>();
		ValidatorOptions.Global.LanguageManager.Enabled = false;

		// Custom persistent config
		services.AddSingleton(OgmaConfig.Init("config.json5"));

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
		services.AddIdentity<OgmaUser, OgmaRole>(config => {
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
			.AddSingleton<ICodeGenerator, CodeGenerator>()
			.AddScoped<UserActivityService>()
			.AddScoped<ETagService>();

		// Claims
		services.AddScoped<IUserClaimsPrincipalFactory<OgmaUser>, OgmaClaimsPrincipalFactory>();
		// services.AddScoped(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User);

		// Argon2 hasher
		services
			.UpgradePasswordSecurity()
			.UseArgon2<OgmaUser>();

		// HttpClient factory
		services.AddHttpClient();

		// Email
		services
			.AddTransient<IEmailSender, PostmarkMailer>()
			.Configure<PostmarkOptions>(configuration);

		// Backblaze
		var b2Options = configuration.GetSection("B2").Get<B2Options>();
		services.AddSingleton<IB2Client>(new B2Client(b2Options));

		// File uploader
		services.AddSingleton<ImageUploader>();

		// Turnstile
		services
			.AddTransient<ITurnstileService, TurnstileService>()
			.Configure<TurnstileSettings>(configuration.GetSection(TurnstileSettings.Section));

		// Seeding
		services.AddAsyncInitializer<DbSeedInitializer>();

		// Auth
		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

		// Auth
		services.AddAuthorizationPolicies();

		// Cookies
		services.ConfigureApplicationCookie(options => {
			options.LoginPath = new PathString("/login");
			options.LogoutPath = new PathString("/logout");
			options.AccessDeniedPath = new PathString("/login");
			options.Cookie.SameSite = SameSiteMode.Lax;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

			options.Events.OnRedirectToLogin = HandleApiRequest(StatusCodes.Status401Unauthorized, options.Events.OnRedirectToLogin);
			options.Events.OnRedirectToAccessDenied = HandleApiRequest(StatusCodes.Status403Forbidden, options.Events.OnRedirectToLogin);
		});

		// Cache
		services.AddMemoryCache();
		services.AddOutputCache(o => {
			o.AddBasePolicy(p => p.Expire(TimeSpan.FromMinutes(5)));
		});
		services.AddFusionCache()
			.WithSerializer(new FusionCacheCysharpMemoryPackSerializer())
			.WithDistributedCache(new RedisCache(new RedisCacheOptions
			{
				Configuration = configuration.GetConnectionString("garnet") ?? "localhost",
				ConfigurationOptions = new()
				{
					DefaultDatabase = GarnetDatabase.Cache,
				}
			}));

		// Runtime compilation
		services
			.AddControllersWithViews();

		// Razor
		services
			.AddRazorPages()
			.AddRazorPagesOptions(options => {
				options.Conventions.AuthorizeAreaFolder("Admin", "/", AuthorizationPolicies.RequireAdminRole);
			});

		// MVC
		services
			.AddMvc(options => {
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
				options.Filters.Add<ValidationExceptionFilter>();
			})
			.AddJsonOptions(options => {
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			});

		// X-CSRF
		services.AddAntiforgery();

		// Json options
		services.ConfigureHttpJsonOptions(options => {
			options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});

		services.Configure<JsonOptions>(options => {
			options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});

		// Fluent Validation
		services
			.AddFluentValidationAutoValidation()
			.AddValidatorsFromAssemblyContaining<Program>()
			.AddFluentValidationClientsideAdapters(cfg => {
				cfg.ClientValidatorFactories[typeof(IFileSizeValidator)] = (_, rule, component) =>
					new FileSizeClientValidator(rule, component);
			});

		// Immediate
		services.AddOgma3Handlers();
		services.AddOgma3Behaviors();

		// Problem details
		services.AddProblemDetails(ProblemDetailsMiddleware.ConfigureProblemDetails);

		// OpenAPI
		services.AddOpenApi("public", options => {
			options.AddOperationTransformer<MinimalApiTagOperationTransformer>();
			options.AddOperationTransformer<IdOperationTransformer>();
			options.AddNullableTransformer();
			options.CreateSchemaReferenceId = NestedSchemaReferenceId.Fun;
			options.ShouldInclude = desc => desc.RelativePath is {} r && !r.StartsWith("admin");
		});
		services.AddOpenApi("internal", options => {
			options.AddOperationTransformer<MinimalApiTagOperationTransformer>();
			options.AddOperationTransformer<IdOperationTransformer>();
			options.AddNullableTransformer();
			options.CreateSchemaReferenceId = NestedSchemaReferenceId.Fun;
			options.ShouldInclude = desc => desc.RelativePath is {} r && r.StartsWith("admin");
		});

		// HSTS
		services.AddHsts(options => {
			options.Preload = true;
			options.IncludeSubDomains = true;
			options.MaxAge = TimeSpan.FromHours(12); // TimeSpan.FromYears(1) when HTTPS config is down pat
		});

		// Rate-limiting profiles
		services.AddRateLimiting();
		// Cache policies
		services.AddCachePolicies();

		return builder;
	}


	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public static void Configure(this WebApplication app)
	{
		var env = app.Environment;

		// Profiler
		if (env.IsDevelopment())
		{
			app.UseMiniProfiler();
		}

		app.MapDefaultEndpoints();

		// Middleware
		app.UseRequestTimestamp();
		app.UseAddHeaders();

		// Compression
		app.UseResponseCompression();

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

		// Forward the IP
		app.UseForwardedHeaders(new ForwardedHeadersOptions
		{
			ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
		});

		// Handle errors
		// TODO: handle it better somehow, using a magic string to discern API endpoints feels iffy at best
		app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"),
			appBuilder => { appBuilder.UseStatusCodePagesWithReExecute("/StatusCode/{0}"); });
		// app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

		// Redirects
		app.UseHttpsRedirection();
		app.UseRewriter(new RewriteOptions()
			.AddRedirect(@"^\.well-known/change-password$", "identity/account/manage/changepassword")
		);

		// Map file extensions
		var extensionsProvider = new FileExtensionContentTypeProvider();
		extensionsProvider.Mappings.Add(".avif", "image/avif");

		// Serve static files with cache headers and compression
		app.UseStaticFiles(new StaticFileOptions
		{
			HttpsCompression = HttpsCompressionMode.Compress,
			OnPrepareResponse = context => {
				context.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
				{
					Public = true,
					MaxAge = TimeSpan.FromDays(365),
				};
				if (context.File.Name.Contains("worker"))
				{
					Log.Information("Serving a service worker");
					context.Context.Response.Headers.Append("Service-Worker-Allowed", "/");
				}
			},
			ContentTypeProvider = extensionsProvider,
		});
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseBanMiddleware();
		app.UseOutputCache();

		// OpenAPI
		app.MapOpenApi("openapi/{documentName}.json").CacheOutput();
		app.MapScalarApiReference();

		// Rate limit
		app.UseRateLimiter();

		app.MapRazorPages();
		app.MapControllers();
		app.MapOgma3Endpoints();

		// Antiforgery
		app.UseAntiforgery();
	}
}