using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Ogma3
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            (string token, string chat) cfg;
            try
            {
                using var sr = new StreamReader("logger-tokens.txt");
                var split = (await sr.ReadToEndAsync()).Split('|');
                cfg.token = split[0];
                cfg.chat = split[1];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Telegram(cfg.token, cfg.chat, restrictedToMinimumLevel: LogEventLevel.Error)
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables("ogma_");
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://+:5001", "https://+:8080", "https://+:80");
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
}
