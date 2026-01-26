namespace Ogma3.Infrastructure.Middleware.RequestTimingMiddleware;

public sealed class RequestTimingMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var feature = new RequestTimingFeature { StartTime = DateTimeOffset.UtcNow };
		context.Features.Set<IRequestTimingFeature>(feature);

		await next(context);
	}
}