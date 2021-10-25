using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogma3.Infrastructure.Logging;
using Serilog;
using Serilog.Events;

namespace Ogma3;

public class Program
{
    public static async Task Main(string[] args)
    {
        // TODO: Reverts to old datetime behaviour, tracked by #50
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";

        var (telegramToken, telegramId) = await Telegram.GetCredentials();
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Telegram(telegramToken, telegramId, restrictedToMinimumLevel: LogEventLevel.Error)
            .WriteTo.Seq(seqUrl, LogEventLevel.Debug)
            .WriteTo.Console(LogEventLevel.Information)
            .MinimumLevel.Debug()
            .CreateLogger();

        try
        {
            var host = CreateHostBuilder(args).Build();
            await host.InitAsync();
            Log.Warning("Genfic has started!");
            await host.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Unexpected shutdown on startup");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddEnvironmentVariables("ogma_");
            })
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("https://+:5001", "https://+:8200", "https://+:80");
                webBuilder.ConfigureKestrel(options =>
                    {
                        options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(10);
                        options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(1);
                        options.ConfigureEndpointDefaults(lo =>
                        {
                            lo.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                    })
                    .UseStartup<Startup>();
            });
    }

}