using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Ogma3.Infrastructure.Middleware;

public sealed class AddHeadersMiddleware(IOptions<AddHeadersOptions> options) : IMiddleware
{
	private readonly AddHeadersOptions _options = options.Value;

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		foreach (var (key, value) in _options.Headers)
		{
			context.Response.Headers.Append(key, value);
		}
		await next(context);
	}
}

public static class AddHeadersMiddlewareExtensions
{
	public static IApplicationBuilder UseAddHeaders(this IApplicationBuilder builder)
		=> builder.UseMiddleware<AddHeadersMiddleware>();

	public static TBuilder UseAddHeaders<TBuilder>(this TBuilder builder)  where TBuilder : IHostApplicationBuilder
	{
		builder.Services
			.AddTransient<AddHeadersMiddleware>()
			.Configure<AddHeadersOptions>(builder.Configuration.GetSection("AdditionalHeaders"));
		return builder;
	}
}

public sealed class AddHeadersOptions
{
	[UsedImplicitly]
	public required Dictionary<string, string> Headers { get; init; }
}