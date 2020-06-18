using System;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    var logger = provider.GetRequiredService<ILogger>();
                    logger.Fatal($"Could not migrate database: {ex.Message}");
                }
            }
            
            await host.InitAsync();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables("ogma_");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://+:5001", "http://+:8080", "http://+:80");
                    webBuilder.ConfigureKestrel(options =>
                        {
                            // Opts
                        })
                        .UseStartup<Startup>();
                });
        }

    }
}
