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
			context.Response.Headers.TryAdd(key, value);
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
			.AddSingleton<AddHeadersMiddleware>()
			.AddOptions<AddHeadersOptions>()
			.Bind(builder.Configuration.GetSection("AdditionalHeaders"))
			.ValidateOnStart();
		return builder;
	}
}

public sealed class AddHeadersOptions
{
	[UsedImplicitly]
	public Dictionary<string, string> Headers { get; init; } = [];
}