namespace Ogma3.Infrastructure.SystemResources;

public static class LinuxMetricsParser
{
	public static CpuTimes? ParseCpuTimes(string contents)
	{
		var cpuLine = contents.Split('\n').FirstOrDefault(line => line.StartsWith("cpu ", StringComparison.Ordinal));
		if (cpuLine is null)
		{
			return null;
		}

		var values = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (values.Length < 5)
		{
			return null;
		}

		var total = 0UL;
		var idle = 0UL;
		for (var i = 1; i < values.Length; i++)
		{
			if (i >= 9)
			{
				continue;
			}

			if (!ulong.TryParse(values[i], out var value))
			{
				return null;
			}

			total += value;
			if (i is 4 or 5)
			{
				idle += value;
			}
		}

		return new CpuTimes(total, idle);
	}

	public static MemoryUsage? ParseMemoryInfo(string contents)
	{
		long? totalBytes = null;
		long? availableBytes = null;
		foreach (var line in contents.Split('\n'))
		{
			var separator = line.IndexOf(':');
			if (separator < 0)
			{
				continue;
			}

			var key = line.AsSpan(0, separator);
			if (key is not "MemTotal" && key is not "MemAvailable")
			{
				continue;
			}

			var value = line.AsSpan(separator + 1).Trim();
			var end = value.IndexOf(' ');
			if (end >= 0)
			{
				value = value[..end];
			}

			if (!long.TryParse(value, out var kilobytes) || kilobytes < 0)
			{
				continue;
			}

			var bytes = kilobytes * 1024;
			if (key is "MemTotal")
			{
				totalBytes = bytes;
			}
			else
			{
				availableBytes = bytes;
			}
		}

		return totalBytes is not null && availableBytes is not null
			? new MemoryUsage(totalBytes.Value, Math.Min(availableBytes.Value, totalBytes.Value))
			: null;
	}

	public static double? CalculateCpuUsage(CpuTimes? previous, CpuTimes? current)
	{
		if (previous is not { } old || current is not { } next || next.Total <= old.Total || next.Idle < old.Idle)
		{
			return null;
		}

		var totalDelta = next.Total - old.Total;
		var idleDelta = next.Idle - old.Idle;
		return Math.Clamp((totalDelta - Math.Min(idleDelta, totalDelta)) * 100d / totalDelta, 0d, 100d);
	}
}

public readonly record struct CpuTimes(ulong Total, ulong Idle);

public readonly record struct MemoryUsage(long TotalBytes, long AvailableBytes)
{
	public long UsedBytes => TotalBytes - AvailableBytes;
}