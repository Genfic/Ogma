using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ogma3.Data;

namespace Ogma3
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;
                try
                {
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    if (configuration.GetValue<bool>("migrateDatabases"))
                    {
                        var identityContext = provider.GetRequiredService<ApplicationDbContext>();
                        await identityContext.Database.MigrateAsync();
                    }
                }
                catch (Exception ex)
                {
                    // var logger = provider.GetRequiredService<ILogger>();
                    // logger.Log(LogLevel.Critical, $"Could not migrate database: {ex.Message}");
                    Console.WriteLine($"Could not migrate database: {ex.Message}");
                }
            }
            
            await host.InitAsync();
            await host.RunAsync();
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://+:5001", "https://+:8080", "https://+:80");
                    webBuilder.ConfigureKestrel(options =>
                        {
                            // Opts
                        })
                        .UseStartup<Startup>();
                });
        }

    }
}
