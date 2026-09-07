using System.Net;
using Immediate.Injections.Shared;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services;

namespace Ogma3.Infrastructure.Middleware;

[RegisterSingleton]
public sealed partial class UserBanMiddleware(BanCache cache, ILogger<UserBanMiddleware> logger) : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (context.User.GetNumericId() is not {} uid)
		{
			await next(context);
			return;
		}

		var allowBanned = context.GetEndpoint()?.Metadata.GetMetadata<AllowBannedUsersAttribute>();
		if (allowBanned is not null)
		{
			await next(context);
			return;
		}

		var isBanned = await cache.Check(uid);

		if (isBanned)
		{
			LogAccessAttempt(logger, uid);

			if (context.IsApiEndpoint())
			{
				context.Response.Clear();
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				await context.Response.WriteAsync("Account banned.", context.RequestAborted);
				return;
			}

			context.Response.Redirect("/Ban");
			return;
		}

		await next(context);
	}

	[LoggerMessage(0, LogLevel.Information, "Banned user {UserId} tried accessing the site")]
	public static partial void LogAccessAttempt(ILogger<UserBanMiddleware> logger, long userId);
}

public static class UserBanMiddlewareExtension
{
	public static IApplicationBuilder UseBanMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<UserBanMiddleware>();
}