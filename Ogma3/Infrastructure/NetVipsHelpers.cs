using NetVips;
using Log = Serilog.Log;

namespace Ogma3.Infrastructure;

public static class NetVipsHelpers
{
	private static readonly int[] VersionIds = [0, 1, 2];

	public static void EnsureInitialized()
	{
		if (ModuleInitializer.VipsInitialized)
		{
			var version = string.Join('.', VersionIds.Select(n => NetVips.NetVips.Version(n)));

			Log.Information("Initialized libvips {Version}", version);
		}
		else
		{
			Log.Error("Libvips initialization failed: {Message}", ModuleInitializer.Exception.Message);
			throw ModuleInitializer.Exception;
		}
	}
}