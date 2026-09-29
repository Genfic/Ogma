using System.Globalization;

namespace Ogma3.Infrastructure.SystemResources;

public static class GarnetMetricsParser
{
	private const string Key = "proc_physical_memory_size:";

	public static long? ParseProcessPhysicalMemory(ReadOnlySpan<char> contents)
	{
		foreach (var line in contents.EnumerateLines())
		{
			var trimmed = line.Trim();
			if (!trimmed.StartsWith(Key, StringComparison.Ordinal))
			{
				continue;
			}

			return long.TryParse(trimmed[Key.Length..], NumberStyles.None, CultureInfo.InvariantCulture, out var bytes)
				? bytes
				: null;
		}

		return null;
	}
}