using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ogma3;
using Ogma3.Infrastructure.Logging;
using Riok.Mapperly.Abstractions;
using Serilog;
using Serilog.Events;

[assembly: MapperDefaults(RequiredMappingStrategy = RequiredMappingStrategy.Target)]

Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-GB");
Console.OutputEncoding = Encoding.UTF8;


var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var (telegramToken, telegramId) = await Telegram.GetCredentials();
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Telegram(telegramToken, telegramId, restrictedToMinimumLevel: LogEventLevel.Error)
	.WriteTo.Seq(seqUrl, LogEventLevel.Debug)
	.WriteTo.Console(LogEventLevel.Information)
	.MinimumLevel.Debug()
	.CreateLogger(); 


var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.ClearProviders()
	.AddConsole();

builder.Configuration
	.AddEnvironmentVariables("ogma_");

builder.Host.UseSerilog();

builder.WebHost.UseUrls("https://+:5001", "https://+:8001", "https://+:80");
builder.WebHost.ConfigureKestrel(options =>
	{
		options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(10);
		options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(1);
		options.ConfigureEndpointDefaults(lo => { lo.Protocols = HttpProtocols.Http1AndHttp2; });
	});

var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

if (args.Any(a => a == "--seed"))
{
	await app.InitAsync();
}

app.Run();