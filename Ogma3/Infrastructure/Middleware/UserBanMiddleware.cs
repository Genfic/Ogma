using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Infrastructure.Middleware;

public sealed class UserBanMiddleware(IMemoryCache cache, ApplicationDbContext dbContext, ILogger<UserBanMiddleware> logger) : IMiddleware
{
	public static string CacheKey(long id) => $"u{id}_Ban";

	public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
	{
		if (httpContext.User.GetNumericId() is not {} uid)
		{
			await next(httpContext);
			return;
		}

		var banDate = await cache.GetOrCreateAsync(CacheKey(uid), async entry =>
		{
			entry.SlidingExpiration = TimeSpan.FromMinutes(30);
			return await dbContext.Infractions
				.TagWith($"{nameof(UserBanMiddleware)} querying for ban status")
				.Where(i => i.UserId == uid)
				.Where(i => i.Type == InfractionType.Ban)
				.Where(i => i.RemovedAt == null)
				.OrderByDescending(i => i.ActiveUntil)
				.Select(i => i.ActiveUntil)
				.FirstOrDefaultAsync();
		});

		if (banDate == default)
		{
			await next(httpContext);
			return;
		}

		if (banDate > DateTime.Now)
		{
			logger.LogInformation("Banned user {UserId} tried accessing the site", uid);
			if (httpContext.Request.Path.StartsWithSegments("/api"))
			{
				httpContext.Response.Clear();
				httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				await httpContext.Response.WriteAsync($"Account banned until {banDate:o}");
			}
			else if (httpContext.Request.Path.StartsWithSegments("/Ban"))
			{
				await next(httpContext);
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
}

public static class UserBanMiddlewareExtension
{
	public static IApplicationBuilder UseBanMiddleware(this IApplicationBuilder builder)
		=> builder.UseMiddleware<UserBanMiddleware>();
}