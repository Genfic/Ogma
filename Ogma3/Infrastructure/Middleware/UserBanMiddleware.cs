using System.Net;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Infrastructure.Extensions;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Infrastructure.Middleware;

public sealed partial class UserBanMiddleware(IFusionCache cache, ApplicationDbContext dbContext, ILogger<UserBanMiddleware> logger) : IMiddleware
{
	public static string CacheKey(long id) => $"user-ban:{id}";

	public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
	{
		if (httpContext.User.GetNumericId() is not {} uid)
		{
			await next(httpContext);
			return;
		}

		var allowBanned = httpContext.GetEndpoint()?.Metadata.GetMetadata<AllowBannedUsersAttribute>();
		if (allowBanned is not null)
		{
			await next(httpContext);
			return;
		}

		var isBanned = await cache.GetOrSetAsync(
			CacheKey(uid),
			async _ => await CompiledQuery(dbContext, uid),
			o => o.Duration = TimeSpan.FromMinutes(30)
		);

		if (isBanned)
		{
			LogAccessAttempt(logger, uid);
			if (httpContext.IsApiEndpoint())
			{
				httpContext.Response.Clear();
				httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				await httpContext.Response.WriteAsync("Account banned.");
			}
			else
			{
				httpContext.Response.Redirect("/Ban");
			}
		}
		else
		{
			await next(httpContext);
		}
	}

	private static readonly Func<ApplicationDbContext, long, Task<bool>> CompiledQuery =
		EF.CompileAsyncQuery(static (ApplicationDbContext dbContext, long uid) => dbContext.Infractions
			.TagWith($"{nameof(UserBanMiddleware)} querying for ban status of user")
			.Where(i => i.UserId == uid)
			.Where(i => i.Type == InfractionType.Ban)
			.Where(i => i.RemovedAt == null)
			.Any(i => i.ActiveUntil > DateTimeOffset.UtcNow)
		);

	[LoggerMessage(0, LogLevel.Information, "Banned user {UserId} tried accessing the site")]
	public static partial void LogAccessAttempt(ILogger<UserBanMiddleware> logger, long userId);
}

public static class UserBanMiddlewareExtension
{
	public static IApplicationBuilder UseBanMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<UserBanMiddleware>();
}