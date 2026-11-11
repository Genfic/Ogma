using Ogma3.Services.PasskeyMetadata;

namespace Ogma3.Infrastructure.Jobs;

public sealed class RefreshPasskeyMetadataRecurringJob
(
	IServiceProvider serviceProvider,
	ILogger<RefreshPasskeyMetadataRecurringJob> logger
) : BaseRecurringJob(serviceProvider, logger)
{
	protected override TimeSpan Interval => TimeSpan.FromDays(1);
	protected override string Name => nameof(RefreshPasskeyMetadataRecurringJob);

	protected override async Task Run(CancellationToken ct)
	{
		var service = ServiceProvider.GetRequiredService<PasskeyMetadataService>();
		await service.RefreshAsync(ct);
	}
}