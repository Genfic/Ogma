using Microsoft.AspNetCore.RateLimiting;

namespace Ogma3.Infrastructure.ServiceRegistrations;

public static class RateLimiting
{
	public const string Rss = nameof(Rss);
	public const string Quotes = nameof(Quotes);
	public const string Reports = nameof(Reports);

	public static IServiceCollection AddRateLimiting(this IServiceCollection services)
	{
		services.AddRateLimiter(x => {
			x.AddFixedWindowLimiter(policyName: Rss, options => {
					options.Window = TimeSpan.FromHours(1);
					options.PermitLimit = 1;
				})
				.AddFixedWindowLimiter(policyName: Quotes, options => {
					options.Window = TimeSpan.FromSeconds(10);
					options.PermitLimit = 1;
				})
				.AddFixedWindowLimiter(policyName: Reports, options => {
					options.Window = TimeSpan.FromHours(1);
					options.PermitLimit = 3;
				});

			x.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
		});
		return services;
	}
}