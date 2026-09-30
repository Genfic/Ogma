using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Immediate.Injections.Shared;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using StackExchange.Redis;

namespace Ogma3.Services;

[RegisterScoped]
public sealed partial class SystemResourceMonitor(AppDbContext context, IConnectionMultiplexer garnet)
{
	private const string GarnetPhysicalMemoryKey = "proc_physical_memory_size:";
	private const string PostgresSizeQuery = """
	                                          SELECT pg_database_size(current_database()) AS "Value"
	                                          """;

	private static readonly TimeSpan SampleDuration = TimeSpan.FromMilliseconds(500);

	public async Task<SystemResourceSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
	{
		using var process = Process.GetCurrentProcess();
		var processCpuBefore = process.TotalProcessorTime;
		var timer = Stopwatch.StartNew();
		var cpuBefore = await ReadCpuTimes(cancellationToken);

		await Task.Delay(SampleDuration, cancellationToken);

		var cpuAfter = await ReadCpuTimes(cancellationToken);
		process.Refresh();
		var processCpuAfter = process.TotalProcessorTime;
		var processCpuUsage = Math.Clamp(
			(processCpuAfter - processCpuBefore).TotalMilliseconds / timer.Elapsed.TotalMilliseconds / Math.Max(1, Environment.ProcessorCount) * 100d,
			0d,
			100d);

		var memory = await ReadMemoryUsage(cancellationToken);
		var disk = ReadDiskUsage();

		return new SystemResourceSnapshot(
			DateTimeOffset.UtcNow,
			CalculateCpuUsage(cpuBefore, cpuAfter),
			memory,
			disk,
			process.WorkingSet64,
			processCpuUsage,
			await ReadGarnetMemoryBytes(cancellationToken),
			await ReadPostgresDatabaseBytes(cancellationToken));
	}

	internal static CpuTimes? ParseCpuTimes(ReadOnlySpan<char> contents)
	{
		foreach (var line in contents.EnumerateLines())
		{
			if (!line.StartsWith("cpu ", StringComparison.Ordinal))
			{
				continue;
			}

			ulong total = 0;
			ulong idle = 0;
			var index = 4;
			for (var field = 0; field < 8; field++)
			{
				while (index < line.Length && line[index] == ' ')
				{
					index++;
				}

				var start = index;
				while (index < line.Length && char.IsAsciiDigit(line[index]))
				{
					index++;
				}

				if (start == index || !ulong.TryParse(line[start..index], out var value))
				{
					return null;
				}

				total += value;
				if (field is 3 or 4)
				{
					idle += value;
				}
			}

			return new CpuTimes(total, idle);
		}

		return null;
	}

	internal static MemoryUsage? ParseMemoryInfo(ReadOnlySpan<char> contents)
	{
		long? totalBytes = null;
		long? availableBytes = null;
		foreach (var line in contents.EnumerateLines())
		{
			var separator = line.IndexOf(':');
			if (separator < 0)
			{
				continue;
			}

			var key = line[..separator];
			if (key is not "MemTotal" && key is not "MemAvailable")
			{
				continue;
			}

			var value = line[(separator + 1)..].Trim();
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

	internal static double? CalculateCpuUsage(CpuTimes? previous, CpuTimes? current)
	{
		if (previous is not { } old || current is not { } next || next.Total <= old.Total || next.Idle < old.Idle)
		{
			return null;
		}

		var totalDelta = next.Total - old.Total;
		var idleDelta = next.Idle - old.Idle;
		return Math.Clamp((totalDelta - Math.Min(idleDelta, totalDelta)) * 100d / totalDelta, 0d, 100d);
	}

	internal static long? ParseGarnetPhysicalMemory(ReadOnlySpan<char> contents)
	{
		foreach (var line in contents.EnumerateLines())
		{
			var trimmed = line.Trim();
			if (!trimmed.StartsWith(GarnetPhysicalMemoryKey, StringComparison.Ordinal))
			{
				continue;
			}

			return long.TryParse(trimmed[GarnetPhysicalMemoryKey.Length..], NumberStyles.None, CultureInfo.InvariantCulture, out var bytes)
				? bytes
				: null;
		}

		return null;
	}

	private static async Task<CpuTimes?> ReadCpuTimes(CancellationToken cancellationToken)
	{
		if (OperatingSystem.IsWindows())
		{
			return ReadWindowsCpuTimes();
		}

		if (!OperatingSystem.IsLinux())
		{
			return null;
		}

		try
		{
			var contents = await File.ReadAllTextAsync("/proc/stat", cancellationToken);
			return ParseCpuTimes(contents);
		}
		catch (IOException)
		{
			return null;
		}
		catch (UnauthorizedAccessException)
		{
			return null;
		}
	}

	private static async Task<MemoryUsage?> ReadMemoryUsage(CancellationToken cancellationToken)
	{
		if (OperatingSystem.IsWindows())
		{
			return ReadWindowsMemoryUsage();
		}

		if (!OperatingSystem.IsLinux())
		{
			return null;
		}

		try
		{
			var contents = await File.ReadAllTextAsync("/proc/meminfo", cancellationToken);
			return ParseMemoryInfo(contents);
		}
		catch (IOException)
		{
			return null;
		}
		catch (UnauthorizedAccessException)
		{
			return null;
		}
	}

	private static DiskUsage? ReadDiskUsage()
	{
		try
		{
			var root = Path.GetPathRoot(AppContext.BaseDirectory) ?? Path.DirectorySeparatorChar.ToString();
			var drive = new DriveInfo(root);
			return drive.IsReady ? new DiskUsage(drive.TotalSize, drive.AvailableFreeSpace) : null;
		}
		catch (IOException)
		{
			return null;
		}
		catch (UnauthorizedAccessException)
		{
			return null;
		}
	}

	private static CpuTimes? ReadWindowsCpuTimes()
	{
		if (!NativeMethods.GetSystemTimes(out var idle, out var kernel, out var user))
		{
			return null;
		}

		return new CpuTimes(kernel.ToUInt64() + user.ToUInt64(), idle.ToUInt64());
	}

	private static MemoryUsage? ReadWindowsMemoryUsage()
	{
		var memory = new MemoryStatus { Length = checked((uint)Marshal.SizeOf<MemoryStatus>()) };
		if (!NativeMethods.GlobalMemoryStatusEx(ref memory))
		{
			return null;
		}

		return new MemoryUsage(checked((long)memory.TotalPhysical), checked((long)memory.AvailablePhysical));
	}

	private async Task<long?> ReadGarnetMemoryBytes(CancellationToken cancellationToken)
	{
		try
		{
			var info = await garnet.GetDatabase().ExecuteAsync("INFO", "memory").WaitAsync(cancellationToken);
			return ParseGarnetPhysicalMemory(info.ToString());
		}
		catch (RedisException)
		{
			return null;
		}
		catch (TimeoutException)
		{
			return null;
		}
	}

	private async Task<long?> ReadPostgresDatabaseBytes(CancellationToken cancellationToken)
	{
		try
		{
			return await context.Database
				.SqlQueryRaw<long>(PostgresSizeQuery)
				.SingleAsync(cancellationToken);
		}
		catch (Npgsql.NpgsqlException)
		{
			return null;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct FileTime
	{
		public uint LowDateTime;
		public uint HighDateTime;

		public readonly ulong ToUInt64() => ((ulong)HighDateTime << 32) | LowDateTime;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct MemoryStatus
	{
		public uint Length;
		public uint MemoryLoad;
		public ulong TotalPhysical;
		public ulong AvailablePhysical;
		public ulong TotalPageFile;
		public ulong AvailablePageFile;
		public ulong TotalVirtual;
		public ulong AvailableVirtual;
		public ulong AvailableExtendedVirtual;
	}

	private static partial class NativeMethods
	{
		[LibraryImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static partial bool GetSystemTimes(out FileTime idle, out FileTime kernel, out FileTime user);

		[LibraryImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static partial bool GlobalMemoryStatusEx(ref MemoryStatus buffer);
	}
}

public sealed record SystemResourceSnapshot(
	DateTimeOffset SampledAt,
	double? CpuUsagePercentage,
	MemoryUsage? Memory,
	DiskUsage? Disk,
	long OgmaWorkingSetBytes,
	double OgmaCpuUsagePercentage,
	long? GarnetMemoryBytes,
	long? PostgresDatabaseBytes
);

[JsonSerializable(typeof(SystemResourceSnapshot))]
[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web)]
public sealed partial class SystemResourceSnapshotContext : JsonSerializerContext;

public readonly record struct CpuTimes(ulong Total, ulong Idle);

public readonly record struct MemoryUsage(long TotalBytes, long AvailableBytes)
{
	public long UsedBytes => TotalBytes - AvailableBytes;
}

public sealed record DiskUsage(long TotalBytes, long AvailableBytes)
{
	public long UsedBytes => TotalBytes - AvailableBytes;
}
