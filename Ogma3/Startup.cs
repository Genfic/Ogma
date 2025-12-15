using System.IO.Compression;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using B2Net;
using B2Net.Models;
using CompressedStaticFiles;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using NpgSqlGenerators;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Notifications;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Compression;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Filters;
using Ogma3.Infrastructure.Middleware;
using Ogma3.Infrastructure.OpenApi;
using Ogma3.Infrastructure.OpenApi.Transformers;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.ServiceDefaults;
using Ogma3.Services;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.ETagService;
using Ogma3.Services.FileLogService;
using Ogma3.Services.FileUploader;
using Ogma3.Services.Initializers;
using Ogma3.Services.Mailer;
using Ogma3.Services.OAuthProviders.Patreon;
using Ogma3.Services.OAuthProviders.Tumblr;
using Ogma3.Services.TurnstileService;
using Ogma3.Services.UserService;
using Scalar.AspNetCore;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.CysharpMemoryPack;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Ogma3;

public static class Startup
{
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
		var conn = configuration.GetConnectionString("ogma3-db");
		services
			.AddDbContext<ApplicationDbContext>(options => options
				.UseNpgsql(conn, o => o
					.MapPostgresEnums()
					.SetPostgresVersion(18, 0))
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

		// Custom persistent config
		services.AddSingleton(OgmaConfig.Init("config.json5"));

		// Comment redirector
		services.AddScoped<CommentRedirector>();

		// Routing
		services.AddRouting(options => options.LowercaseUrls = true);

		// HttpContextAccessor
		services.AddHttpContextAccessor();

		// ActionContextAccessor
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
			.AddScoped<ETagService>()
			.AddSingleton<IFileLogService, FileLogService>()
			.Configure<FileLogOptions>(c => {
				c.MaxSizeInBytes = 50 * 1024 * 1024;
				c.Directory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
			});

		// Claims
		services.AddScoped<IUserClaimsPrincipalFactory<OgmaUser>, OgmaClaimsPrincipalFactory>();
		// services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
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
			.Configure<PostmarkOptions>(configuration.GetSection("Postmark"));

		// Backblaze
		var b2Options = configuration.GetSection("B2").Get<B2Options>() ?? throw new InvalidOperationException("B2 options not found");
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
		services
			.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddPatreon(options => configuration.Bind("Authentication:Patreon", options))
			.AddTumblr(options => configuration.Bind("Authentication:Tumblr", options))
			.AddGoogle(options => {
				configuration.Bind("Authentication:Google", options);
				options.CallbackPath = "/oauth/google";
				options.ClaimActions.MapJsonKey(ClaimTypes.Avatar, "picture");
			});

		// Auth
		services.AddAuthorizationPolicies();

		// Cookies
		services.ConfigureApplicationCookie(options => {
			options.LoginPath = new PathString("/login");
			options.LogoutPath = new PathString("/logout");
			options.AccessDeniedPath = new PathString("/login");
			options.Cookie.SameSite = SameSiteMode.Lax;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		});

		// Cache
		services.AddMemoryCache();
		services.AddOutputCache();
		builder.AddRedisDistributedCache(connectionName: "garnet");
		services.AddStackExchangeRedisCache(o => {
			o.Configuration = configuration.GetConnectionString("garnet") ?? "localhost";
			o.ConfigurationOptions = new()
			{
				DefaultDatabase = GarnetDatabase.Cache,
			};
		});
		services.AddFusionCache()
			.WithSerializer(new FusionCacheCysharpMemoryPackSerializer())
			.WithRegisteredDistributedCache();

		// Precompressed files
		services.AddCompressedStaticFiles();

		static void ConfigJson(JsonSerializerOptions options)
		{
			options.Converters.Add(new JsonStringEnumConverter());
			options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			options.NumberHandling = JsonNumberHandling.Strict;
		}

		// Json options
		services.ConfigureHttpJsonOptions(options => ConfigJson(options.SerializerOptions));

		// MVC
		services
			.AddMvc(options => {
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
				options.Filters.Add<ValidationExceptionFilter>();
			})
			.AddJsonOptions(options => ConfigJson(options.JsonSerializerOptions));

		// Razor
		services
			.AddRazorPages(options => {
				options.Conventions.AuthorizeAreaFolder("Admin", "/", AuthorizationPolicies.RequireAdminRole);
			});

		services.AddSession();

		// X-CSRF
		services.AddAntiforgery();

		// Fluent Validation
		services
			.AddFluentValidationAutoValidation()
			.AddValidatorsFromAssemblyContaining<Program>()
			.AddFluentValidationClientsideAdapters(cfg => {
				cfg.ClientValidatorFactories[typeof(IFileSizeValidator)] = (_, rule, component) =>
					new FileSizeClientValidator(rule, component);
			});
		ValidatorOptions.Global.LanguageManager.Enabled = false;

		// Immediate
		services.AddOgma3Handlers();
		services.AddOgma3Behaviors();

		// Problem details
		services.AddProblemDetails(ProblemDetailsMiddleware.ConfigureProblemDetails);

		// OpenAPI
		services.AddOpenApi("public", options => {
			options.AddOperationTransformer<MinimalApiTagOperationTransformer>();
			options.AddOperationTransformer<IdOperationTransformer>();
			// options.AddNullableTransformer();
			options.CreateSchemaReferenceId = NestedSchemaReferenceId.Fun;
			options.ShouldInclude = desc => desc.RelativePath is {} r && !r.StartsWith("admin");
		});
		services.AddOpenApi("internal", options => {
			options.AddOperationTransformer<MinimalApiTagOperationTransformer>();
			options.AddOperationTransformer<IdOperationTransformer>();
			// options.AddNullableTransformer();
			options.CreateSchemaReferenceId = NestedSchemaReferenceId.Fun;
			options.ShouldInclude = desc => desc.RelativePath is {} r && r.StartsWith("admin");
		});

		// HSTS
		services.AddHsts(options => {
			options.Preload = true;
			options.IncludeSubDomains = true;
			options.MaxAge = TimeSpan.FromHours(12); // TimeSpan.FromYears(1) when HTTPS config is down pat
		});

		// Security headers
		services.AddCustomSecurityHeaderPolicies();

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
			appBuilder => appBuilder.UseStatusCodePagesWithReExecute("/StatusCode/{0}"));

		// app.UseWhen(context => {
		// 		if (context.GetEndpoint() is not {} endpoint) return true;
		// 		return endpoint.Metadata.GetMetadata<IApiBehaviorMetadata>() is null;
		// 	},
		// 	appBuilder => appBuilder.UseStatusCodePagesWithReExecute("/StatusCode/{0}"));

		// app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

		// Redirects
		app.UseHttpsRedirection();
		app.UseRewriter(new RewriteOptions()
			.AddRedirect(@"^\.well-known/change-password$", "identity/account/manage/changepassword")
		);

		// Serve static files
		app.UseCustomStaticFiles();
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseBanMiddleware();
		app.UseOutputCache();

		app.UseSession();

		// OpenAPI
		app.MapOpenApi("openapi/{documentName}.json");
		app.MapScalarApiReference().WithSecurityHeadersPolicy(SecurityHeaderPolicies.Lax);

		// Rate limit
		app.UseRateLimiter();

		app.MapRazorPages();
		app.MapControllers();
		app.MapOgma3Endpoints();

		// Antiforgery
		app.UseAntiforgery();

		// Security headers
		app.UseSecurityHeaders();
	}
}