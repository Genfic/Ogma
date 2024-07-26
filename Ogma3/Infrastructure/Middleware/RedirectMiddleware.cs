using Microsoft.Extensions.Options;

namespace Ogma3.Infrastructure.Middleware;

public class RedirectMiddleware(RequestDelegate next, IOptions<RedirectMiddlewareOptions> options, ILogger<RedirectMiddleware> logger)
{
	private readonly RedirectMiddlewareOptions _options = options.Value;

	public async Task InvokeAsync(HttpContext context)
	{
		if (_options.Redirects.TryGetValue(context.Request.Path, out var redirect))
		{
			logger.LogInformation("Redirecting from {Source} to {Target}", context.Request.Path, redirect);
			context.Response.Redirect(redirect);
			return;
		}

		await next(context);
	}
}

public static class RedirectMiddlewareExtensions
{
	public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder builder, Action<RedirectMiddlewareOptions> config)
	{
		var options = new RedirectMiddlewareOptions();
		config(options);
		return builder.UseMiddleware<RedirectMiddleware>(Options.Create(options));
	}
}

public class RedirectMiddlewareOptions
{
	public Dictionary<string, string> Redirects { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}