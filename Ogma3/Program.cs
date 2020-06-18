using System.Threading.Tasks;
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
                catch
                {
                    // ignored
                }
            }
            
            await host.InitAsync();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                        {
                            // Opts
                        })
                        .UseStartup<Startup>();
//                    webBuilder.UseStartup<Startup>();
                });
        }

    }
}
