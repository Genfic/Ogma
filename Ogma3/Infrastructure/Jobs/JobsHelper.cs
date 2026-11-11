using Ogma3.Services.PasskeyMetadata;

namespace Ogma3.Infrastructure.Jobs;

public static class JobsHelper
{
	public static IServiceCollection RegisterJobs(this IServiceCollection services)
	{
		return services
			.AddHostedService<PasskeyMetadataInitializer>()
			.AddHostedService<UnbanRecurringJob>()
			.AddHostedService<DeleteInactiveAccountsRecurringJob>()
			.AddHostedService<DeleteDraftsRecurringJob>()
			.AddHostedService<DeleteStaleNotificationsRecurringJob>()
			.AddHostedService<ReconcileBanCacheRecurringJob>()
			.AddHostedService<PublishScheduledContentRecurringJob>()
			.AddHostedService<ProcessSoftDeletesRecurringJob>()
			.AddHostedService<PurgeRevokedSubscriptionsRecurringJob>()
			.AddHostedService<RefreshPasskeyMetadataRecurringJob>()
			;
	}
}