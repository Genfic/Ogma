using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Ogma3.Infrastructure.ServiceRegistrations;

public static class RateLimiting
{
	public const string Rss = nameof(Rss);
	public const string Quotes = nameof(Quotes);
	public const string Reports = nameof(Reports);

	public static IServiceCollection AddRateLimiting(this IServiceCollection services)
	{
		services.AddRateLimiter(limiterOptions => {
			limiterOptions.AddFixedWindowLimiter(policyName: Rss, options => {
					options.Window = TimeSpan.FromHours(1);
					options.PermitLimit = 1;
				})
				.AddFixedWindowLimiter(policyName: Quotes, options => {
					options.Window = TimeSpan.FromSeconds(5);
					options.PermitLimit = 1;
				})
				.AddFixedWindowLimiter(policyName: Reports, options => {
					options.Window = TimeSpan.FromHours(1);
					options.PermitLimit = 3;
				});

			limiterOptions.OnRejected = (OnRejectedContext context, CancellationToken _) => {
				if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
				{
					context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString("#");
				}

				context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

				return new ValueTask();
			};
		});
		
		return services;
	}
}