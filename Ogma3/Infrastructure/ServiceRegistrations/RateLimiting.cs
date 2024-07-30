using Microsoft.AspNetCore.RateLimiting;

namespace Ogma3.Infrastructure.ServiceRegistrations;

public static class RateLimiting
{
	public const string Rss = nameof(Rss);
	public const string Quotes = nameof(Quotes);

	public static IServiceCollection AddRateLimiting(this IServiceCollection services)
	{
		return services.AddRateLimiter(x => x
			.AddFixedWindowLimiter(policyName: Rss, options => {
				options.Window = TimeSpan.FromHours(1);
			})
			.AddFixedWindowLimiter(policyName: Quotes, options => {
				options.Window = TimeSpan.FromSeconds(10);
			})
		);
	}
}