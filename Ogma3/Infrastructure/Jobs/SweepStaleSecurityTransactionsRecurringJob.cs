using Ogma3.Services.Security;

namespace Ogma3.Infrastructure.Jobs;

public sealed class SweepStaleSecurityTransactionsRecurringJob(
	IServiceProvider serviceProvider,
	ILogger<SweepStaleSecurityTransactionsRecurringJob> logger)
	: BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromHours(1);
	protected override string Name => nameof(SweepStaleSecurityTransactionsRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		using var scope = ServiceProvider.CreateScope();
		var service = scope.ServiceProvider.GetRequiredService<SecurityAuthorizationService>();

		var swept = await service.SweepAsync(ct);
		if (swept > 0)
		{
			Logger.LogInformation("Swept {Count} stale security transactions.", swept);
		}
	}
}
