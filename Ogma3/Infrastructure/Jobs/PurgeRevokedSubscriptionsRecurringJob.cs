using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Jobs;

/// <summary>
///     Hard-deletes subscriptions whose revocation grace period has lapsed. Until this runs, a
///     revoked patron keeps their entitlements — and therefore their draft retention window —
///     so a failed payment or a spurious webhook cannot cost them their drafts.
/// </summary>
public sealed class PurgeRevokedSubscriptionsRecurringJob
	(IServiceProvider serviceProvider, OgmaConfig.OgmaConfig config, ILogger<PurgeRevokedSubscriptionsRecurringJob> logger)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromDays(1);
	protected override string Name => nameof(PurgeRevokedSubscriptionsRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var cutoff = DateTimeOffset.UtcNow.AddDays(-config.EntitlementGraceDays);

		var purged = await ctx.Subscriptions
			.Where(s => s.RevokedAt != null && s.RevokedAt <= cutoff)
			.ExecuteDeleteAsync(ct);

		if (purged > 0)
		{
			logger.LogInformation("Purged {Purged} subscriptions revoked before {Cutoff}", purged, cutoff);
		}
	}
}
