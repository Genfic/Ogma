using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Extensions;
using Serilog;

namespace Ogma3.Infrastructure.Middleware;

public class UserBanMiddleware(IMemoryCache cache, ApplicationDbContext dbContext) : IMiddleware
{
	public static string CacheKey(long id) => $"u{id}_Ban";

	public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
	{
		var uid = httpContext.User.GetNumericId();
		if (uid is null)
		{
			await next(httpContext);
			return;
		}

		var banDate = await cache.GetOrCreateAsync(CacheKey((long)uid), async entry =>
		{
			entry.SlidingExpiration = TimeSpan.FromMinutes(30);
			return await dbContext.Infractions
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
			Log.Information("Banned user {UserId} tried accessing the site", uid);
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