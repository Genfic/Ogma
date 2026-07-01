using ConfigBinder.Attributes;
using Immediate.Validations.Shared;
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
			.AddSingleton<AddHeadersMiddleware>();
		return builder;
	}
}

[Validate]
[ConfigSection("AdditionalHeaders")]
public sealed partial class AddHeadersOptions : IValidationTarget<AddHeadersOptions>
{
	[UsedImplicitly]
	public Dictionary<string, string> Headers { get; init; } = [];
}