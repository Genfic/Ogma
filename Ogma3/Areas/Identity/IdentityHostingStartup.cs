using Microsoft.AspNetCore.Hosting;
using Ogma3.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace Ogma3.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
	public void Configure(IWebHostBuilder builder)
	{
		builder.ConfigureServices((_, _) => { });
	}
}