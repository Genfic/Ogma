namespace Ogma3.Infrastructure.Middleware.RequestTimingMiddleware;

public static class RequestTimestampMiddlewareExtensions
{
	public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
		=> builder.UseMiddleware<RequestTimingMiddleware>();

	public static TimeSpan? GetRequestDuration(this HttpContext context)
		=> context.Features.Get<IRequestTimingFeature>()?.Duration;
}