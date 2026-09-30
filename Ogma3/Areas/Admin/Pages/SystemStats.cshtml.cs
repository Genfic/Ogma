using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Infrastructure.SystemResources;
using Ogma3.Services;
using StackExchange.Redis;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireStaffRole)]
public sealed class SystemStats(SystemResourceMonitor monitor, AppDbContext context, IConnectionMultiplexer garnet) : PageModel
{
	private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

	public required SystemResourceSnapshot Snapshot { get; set; }
	public long? GarnetMemoryBytes { get; private set; }
	public long? PostgresDatabaseBytes { get; private set; }
	public DateTimeOffset SampledAt { get; private set; }
	public string HostCpuUsage => Snapshot.CpuUsagePercentage is { } cpu ? $"{cpu:N1}%" : "Unavailable on this platform";
	public string HostMemoryUsed => Snapshot.Memory is { TotalBytes: > 0 } memory
		? $"{FormatBytes(memory.UsedBytes)} / {FormatBytes(memory.TotalBytes)} ({memory.UsedBytes * 100d / memory.TotalBytes:N1}%)"
		: "Unavailable on this platform";
	public string HostMemoryAvailable => Snapshot.Memory is { } memory ? FormatBytes(memory.AvailableBytes) : "Unavailable";
	public string HostDiskUsed => Snapshot.Disk is { TotalBytes: > 0 } disk
		? $"{FormatBytes(disk.UsedBytes)} / {FormatBytes(disk.TotalBytes)} ({disk.UsedBytes * 100d / disk.TotalBytes:N1}%)"
		: "Unavailable";
	public string HostDiskAvailable => Snapshot.Disk is { } disk ? FormatBytes(disk.AvailableBytes) : "Unavailable";
	public string OgmaWorkingSet => FormatBytes(Snapshot.OgmaWorkingSetBytes);
	public string OgmaCpuUsage => $"{Snapshot.OgmaCpuUsagePercentage:N1}%";
	public string GarnetMemory => GarnetMemoryBytes is { } bytes ? FormatBytes(bytes) : "Unavailable";
	public string PostgresDatabaseSize => PostgresDatabaseBytes is { } bytes ? FormatBytes(bytes) : "Unavailable";

	public async Task OnGetAsync(CancellationToken cancellationToken)
	{
		await RefreshStats(cancellationToken);
	}

	public async Task OnGetStreamAsync(CancellationToken cancellationToken)
	{
		Response.ContentType = "text/event-stream";
		Response.Headers.CacheControl = "no-cache";
		Response.Headers.Append("X-Accel-Buffering", "no");

		using var timer = new PeriodicTimer(UpdateInterval);
		do
		{
			await RefreshStats(cancellationToken);
			var update = new SystemStatsUpdate(
				SampledAt,
				HostCpuUsage,
				HostMemoryUsed,
				HostMemoryAvailable,
				HostDiskUsed,
				HostDiskAvailable,
				OgmaWorkingSet,
				OgmaCpuUsage,
				GarnetMemory,
				PostgresDatabaseSize);

			await Response.WriteAsync($"data: {JsonSerializer.Serialize(update, JsonSerializerOptions.Web)}\n\n", cancellationToken);
			await Response.Body.FlushAsync(cancellationToken);
		}
		while (await timer.WaitForNextTickAsync(cancellationToken));
	}

	private async Task RefreshStats(CancellationToken cancellationToken)
	{
		Snapshot = await monitor.GetSnapshotAsync(cancellationToken);
		SampledAt = DateTimeOffset.UtcNow;
		GarnetMemoryBytes = await GetGarnetMemory(cancellationToken);
		PostgresDatabaseBytes = await GetPostgresDatabaseSize(cancellationToken);
	}

	private static string FormatBytes(long bytes) => Humanizer.ByteSize.FromBytes(bytes).ToString();

	private async Task<long?> GetGarnetMemory(CancellationToken cancellationToken)
	{
		try
		{
			var info = await garnet.GetDatabase().ExecuteAsync("INFO", "memory").WaitAsync(cancellationToken);
			return GarnetMetricsParser.ParseProcessPhysicalMemory(info.ToString());
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

	private async Task<long?> GetPostgresDatabaseSize(CancellationToken cancellationToken)
	{
		try
		{
			return await context.Database
				.SqlQueryRaw<long>("""
				                   SELECT pg_database_size(current_database()) AS "Value"
				                   """)
				.SingleAsync(cancellationToken);
		}
		catch (Npgsql.NpgsqlException)
		{
			return null;
		}
	}

	private sealed record SystemStatsUpdate(
		DateTimeOffset SampledAt,
		string HostCpuUsage,
		string HostMemoryUsed,
		string HostMemoryAvailable,
		string HostDiskUsed,
		string HostDiskAvailable,
		string OgmaWorkingSet,
		string OgmaCpuUsage,
		string GarnetMemory,
		string PostgresDatabaseSize);
}