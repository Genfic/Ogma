using System.Diagnostics;
using System.Runtime.InteropServices;
using Immediate.Injections.Shared;
using Ogma3.Infrastructure.SystemResources;

namespace Ogma3.Services;

[RegisterScoped]
public sealed partial class SystemResourceMonitor
{
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
			LinuxMetricsParser.CalculateCpuUsage(cpuBefore, cpuAfter),
			memory,
			disk,
			process.WorkingSet64,
			processCpuUsage);
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
			return LinuxMetricsParser.ParseCpuTimes(contents);
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
			return LinuxMetricsParser.ParseMemoryInfo(contents);
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
	double? CpuUsagePercentage,
	MemoryUsage? Memory,
	DiskUsage? Disk,
	long OgmaWorkingSetBytes,
	double OgmaCpuUsagePercentage
);

public sealed record DiskUsage(long TotalBytes, long AvailableBytes)
{
	public long UsedBytes => TotalBytes - AvailableBytes;
}