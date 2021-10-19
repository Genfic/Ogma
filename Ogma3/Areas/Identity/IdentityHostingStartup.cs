using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Ogma3.Areas.Identity.IdentityHostingStartup))]
namespace Ogma3.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((_, _) => {
        });
    }
}