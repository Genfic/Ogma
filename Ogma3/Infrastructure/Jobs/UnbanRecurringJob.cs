using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;

namespace Ogma3.Infrastructure.Jobs;

public sealed class UnbanRecurringJob(IServiceProvider serviceProvider, ILogger<UnbanRecurringJob> logger)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromHours(1);
	protected override string Name => nameof(UnbanRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		var count = await ctx.Infractions
			.Where(i => i.Type == InfractionType.Ban)
			.Where(i => i.RemovedAt == null)
			.Where(i => i.ActiveUntil < DateTimeOffset.UtcNow)
			.ExecuteUpdateAsync(setter => setter
					.SetProperty(i => i.RemovedAt, DateTimeOffset.UtcNow),
				cancellationToken: ct);

		Logger.LogInformation("Unbanned {Count} users", count);
	}
}