namespace Ogma3.Infrastructure.Middleware;

public sealed class RequestTimestampMiddleware : IMiddleware
{
	public const string Name = "RequestStartedOn";

	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		context.Items.Add(Name, DateTimeOffset.UtcNow);
		return next(context);
	}
}

public static class RequestTimestampMiddlewareExtensions
{
	public static IApplicationBuilder UseRequestTimestamp(this IApplicationBuilder builder)
		=> builder.UseMiddleware<RequestTimestampMiddleware>();
}