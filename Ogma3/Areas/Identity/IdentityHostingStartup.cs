using Ogma3.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace Ogma3.Areas.Identity;

public sealed class IdentityHostingStartup : IHostingStartup
{
	public void Configure(IWebHostBuilder builder)
	{
		builder.ConfigureServices((_, _) => { });
	}
}