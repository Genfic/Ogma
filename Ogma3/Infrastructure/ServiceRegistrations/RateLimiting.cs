using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Infrastructure.ServiceRegistrations;

public static class RateLimiting
{
	public const string Rss = nameof(Rss);
	public const string Quotes = nameof(Quotes);
	public const string Reports = nameof(Reports);
	public const string Registration = nameof(Registration);
	public const string Login = nameof(Login);

	public static IServiceCollection AddRateLimiting(this IServiceCollection services)
	{
		services.AddRateLimiter(limiterOptions => {
			limiterOptions
				.AddFixedWindowLimiter(policyName: Rss, options => {
					options.Window = TimeSpan.FromHours(1);
					options.PermitLimit = 1;
				})
				.AddFixedWindowLimiter(policyName: Quotes, options => {
					options.Window = TimeSpan.FromSeconds(5);
					options.PermitLimit = 1;
				})
				.AddPartitionedFixedWindowLimiter(policyName: Reports, options => {
					options.Window = TimeSpan.FromHours(1);
					options.PermitLimit = 5;
				})
				.AddPartitionedSlidingWindowLimiter(policyName: Registration, options => {
					options.Window = TimeSpan.FromHours(2);
					options.PermitLimit = 5;
					options.SegmentsPerWindow = 5;
					options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
					options.QueueLimit = 0;
				})
				.AddPartitionedSlidingWindowLimiter(policyName: Login, options => {
					options.Window = TimeSpan.FromMinutes(30);
					options.PermitLimit = 10;
					options.SegmentsPerWindow = 10;
					options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
					options.QueueLimit = 0;
				});

			limiterOptions.OnRejected = (context, _) => {
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