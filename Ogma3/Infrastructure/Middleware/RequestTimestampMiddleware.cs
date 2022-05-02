using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Infrastructure.Middleware;

public class RequestTimestampMiddleware : IMiddleware
{
	public const string Name = "RequestStartedOn";

	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		context.Items.Add(Name, DateTime.UtcNow);
		return next(context);
	}
}

public static class RequestTimestampMiddlewareExtensions
{
	public static IApplicationBuilder UseRequestTimestamp(this IApplicationBuilder builder)
		=> builder.UseMiddleware<RequestTimestampMiddleware>();
}