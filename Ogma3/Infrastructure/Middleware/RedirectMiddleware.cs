using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Ogma3.Infrastructure.Middleware;

public sealed partial class RedirectMiddleware(RequestDelegate next, IOptions<RedirectMiddlewareOptions> options, ILogger<RedirectMiddleware> logger)
{
	private readonly RedirectMiddlewareOptions _options = options.Value;

	public async Task InvokeAsync(HttpContext context)
	{
		if (_options.Redirects.TryGetValue(context.Request.Path, out var redirect))
		{
			LogRedirect(logger, context.Request.Path, redirect);
			context.Response.Redirect(redirect);
			return;
		}

		await next(context);
	}
	
	[LoggerMessage(0, LogLevel.Information, "Redirecting from {Source} to {Target}")]
	public static partial void LogRedirect(ILogger<RedirectMiddleware> logger, PathString source, string target);
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

public sealed class RedirectMiddlewareOptions
{
	[UsedImplicitly]
	public Dictionary<string, string> Redirects { get; } = new(StringComparer.OrdinalIgnoreCase);
}