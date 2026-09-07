using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Services;

namespace Ogma3.Infrastructure.Jobs;

public sealed class ReconcileBanCacheRecurringJob
(
	IServiceProvider serviceProvider,
	BanCache banCache,
	ILogger<ReconcileBanCacheRecurringJob> logger
)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromHours(1);
	protected override string Name => nameof(ReconcileBanCacheRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var bans = await ctx.Infractions
			.Where(i => i.Type == InfractionType.Ban)
			.Where(i => i.ActiveUntil > DateTimeOffset.UtcNow)
			.Select(i => new BanCache.ActiveBan(i.UserId, i.ActiveUntil))
			.ToListAsync(ct);

		_ = await banCache.Reconcile(bans);
	}
}