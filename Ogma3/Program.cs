using System.Globalization;
using System.Reflection;
using System.Text;
using AutoSeal;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Ogma3;
using Ogma3.Data;
using Ogma3.ServiceDefaults;
using Riok.Mapperly.Abstractions;
using Serilog;
using Serilog.Events;

[assembly: MapperDefaults(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[assembly: Behaviors(typeof(ValidationBehavior<,>))]
[assembly: SealPublicClasses]

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
Console.OutputEncoding = Encoding.UTF8;

var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Seq(seqUrl, LogEventLevel.Debug)
	.WriteTo.Console(LogEventLevel.Information)
	.WriteTo.OpenTelemetry()
	.MinimumLevel.Debug()
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddEnvironmentVariables("ogma_")
	.AddJsonFile("appsettings.json5")
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json5", true)
	.AddEnvironmentVariables()
	// WARN: It probably should not be used in prod, switch to DI instead
	.AddUserSecrets(Assembly.GetAssembly(typeof(Program)) ?? throw new NullReferenceException("The assembly was, somehow, null"));


builder.Logging
	.ClearProviders()
	.AddConsole();

builder.Host.UseSerilog();

// builder.WebHost.UseUrls("https://+:5001");
builder.WebHost.ConfigureKestrel(options =>
	{
		options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(10);
		options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(1);
		options.ConfigureEndpointDefaults(lo => { lo.Protocols = HttpProtocols.Http1AndHttp2AndHttp3; });
	});

await builder.ConfigureServices();

builder.AddServiceDefaults();
builder.AddRedisClient("garnet");

var app = builder.Build();

app.Configure();

if (app.Environment.IsDevelopment())
{
	using var serviceScope = app.Services.CreateScope();
	var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	await dbContext.Database.MigrateAsync();
}

if (builder.Configuration.GetValue<bool>("SHOULD_SEED"))
{
	await app.InitAsync();
}

app.Run();