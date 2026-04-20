namespace Ogma3.Infrastructure.Jobs;

public static class JobsHelper
{
	public static IServiceCollection RegisterJobs(this IServiceCollection services)
	{
		return services
			.AddHostedService<UnbanRecurringJob>()
			.AddHostedService<DeleteInactiveAccountsRecurringJob>()
			;
	}
}