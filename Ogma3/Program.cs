using System.Globalization;
using System.Reflection;
using System.Text;
using AutoSeal;
using Immediate.Handlers.Shared;
using Immediate.Injections.Shared;
using Immediate.Validations.Shared;
using InfisicalConfig;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ogma3;
using Ogma3.Infrastructure;
using Ogma3.ServiceDefaults;
using Riok.Mapperly.Abstractions;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: MapperDefaults(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[assembly: Behaviors(typeof(ValidationBehavior<,>))]
[assembly: SealPublicClasses]
[assembly: RegistrationDefaults(RegistrationStrategy = RegistrationStrategy.SelfAndImplementedInterfaces)]

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
Console.OutputEncoding = Encoding.Unicode;

NetVipsHelpers.EnsureInitialized();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddEnvironmentVariables("ogma_")
	.AddJsonFile("appsettings.json5")
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json5", true);

builder.Configuration.AddUserSecrets(Assembly.GetAssembly(typeof(Program))
                                     ?? throw new NullReferenceException("The assembly was, somehow, null"));

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.MinimumLevel.Debug()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.Logger(lc => lc
		.ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions { SectionName = "ConsoleLogger" })
		.WriteTo.Console(LogEventLevel.Information, theme: ConsoleTheme.None))
	.WriteTo.OpenTelemetry()
	.CreateLogger();

await builder.AddInfisicalAsync();

builder.Host.UseSerilog();

// builder.WebHost.UseUrls("https://+:5001");
builder.WebHost.ConfigureKestrel(options => {
	options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(10);
	options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(1);
	options.ConfigureEndpointDefaults(lo => { lo.Protocols = HttpProtocols.Http1AndHttp2AndHttp3; });
});

builder.AddRedisClient("garnet");

await builder.ConfigureServices();

builder.AddServiceDefaults();

var app = builder.Build();

app.Configure();

await app.RunAsync();