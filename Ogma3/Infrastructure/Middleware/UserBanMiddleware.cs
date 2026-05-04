using System.Net;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Attributes;
using Ogma3.Infrastructure.Extensions;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Infrastructure.Middleware;

[RegisterTransient]
public sealed partial class UserBanMiddleware(IFusionCache cache, ILogger<UserBanMiddleware> logger) : IMiddleware
{
	public static string CacheKey(long id) => $"user-ban:{id}";

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

		var isBanned = await cache.GetOrSetAsync(
			CacheKey(uid),
			async _ => {
				var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
				return await CompiledQuery(dbContext, uid);
			},
			o => o.Duration = TimeSpan.FromMinutes(30)
		);

		if (isBanned)
		{
			LogAccessAttempt(logger, uid);

			if (context.IsApiEndpoint())
			{
				context.Response.Clear();
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				await context.Response.WriteAsync("Account banned.");
				return;
			}

			context.Response.Redirect("/Ban");
			return;
		}

		await next(context);
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