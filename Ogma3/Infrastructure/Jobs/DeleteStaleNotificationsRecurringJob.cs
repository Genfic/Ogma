using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Jobs;

public sealed class DeleteStaleNotificationsRecurringJob(IServiceProvider serviceProvider, ILogger logger)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromDays(1);
	protected override string Name => nameof(DeleteStaleNotificationsRecurringJob);
	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		var staleRows = await ctx.Notifications
			.Where(n => n.DateTime < DateTimeOffset.UtcNow.AddMonths(-2))
			.ExecuteDeleteAsync(ct);

		Logger.LogInformation("Deleted {Count} stale notifications.", staleRows);

		var orphanRows = await ctx.Notifications
			.Where(n => !n.Recipients.Any())
			.ExecuteDeleteAsync(ct);

		Logger.LogInformation("Deleted {Count} orphaned notifications.", orphanRows);
	}
}