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
	public required SystemResourceSnapshot Snapshot { get; set; }
	public long? GarnetMemoryBytes { get; private set; }
	public long? PostgresDatabaseBytes { get; private set; }
	public DateTimeOffset SampledAt { get; private set; }

	public async Task OnGetAsync(CancellationToken cancellationToken)
	{
		Snapshot = await monitor.GetSnapshotAsync(cancellationToken);
		SampledAt = DateTimeOffset.UtcNow;
		GarnetMemoryBytes = await GetGarnetMemory(cancellationToken);
		PostgresDatabaseBytes = await GetPostgresDatabaseSize(cancellationToken);
	}

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
}