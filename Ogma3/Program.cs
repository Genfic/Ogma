using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Ogma3;
using Ogma3.Data;
using Ogma3.Infrastructure.Middleware;
using Ogma3.ServiceDefaults;
using Riok.Mapperly.Abstractions;
using Serilog;
using Serilog.Events;

[assembly: MapperDefaults(RequiredMappingStrategy = RequiredMappingStrategy.Target)]

Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-GB");
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

var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

builder.AddServiceDefaults();

// middleware
builder.UseAddHeaders();

var app = builder.Build();

Startup.Configure(app, app.Environment);

// middleware
app.UseAddHeaders();

if (app.Environment.IsDevelopment())
{
	using var serviceScope = app.Services.CreateScope();
	var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	await dbContext.Database.MigrateAsync();
}

if (args.Any(a => a == "--seed"))
{
	await app.InitAsync();
}

app.Run();