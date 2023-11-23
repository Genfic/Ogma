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

public class UserBanMiddleware(IMemoryCache cache, ApplicationDbContext context) : IMiddleware
{
	public static string CacheKey(long id) => $"u{id}_Ban";

	public async Task InvokeAsync(HttpContext context1, RequestDelegate next)
	{
		var uid = context1.User.GetNumericId();
		if (uid is null)
		{
			await next(context1);
			return;
		}

		var banDate = await cache.GetOrCreateAsync(CacheKey((long)uid), async entry =>
		{
			entry.SlidingExpiration = TimeSpan.FromMinutes(30);
			return await context.Infractions
				.Where(i => i.UserId == uid)
				.Where(i => i.Type == InfractionType.Ban)
				.Where(i => i.RemovedAt == null)
				.OrderByDescending(i => i.ActiveUntil)
				.Select(i => i.ActiveUntil)
				.FirstOrDefaultAsync();
		});

		if (banDate == default)
		{
			await next(context1);
			return;
		}

		if (banDate > DateTime.Now)
		{
			Log.Information("Banned user {UserId} tried accessing the site", uid);
			if (context1.Request.Path.StartsWithSegments("/api"))
			{
				context1.Response.Clear();
				context1.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				await context1.Response.WriteAsync($"Account banned until {banDate:o}");
			}
			else if (context1.Request.Path.StartsWithSegments("/Ban"))
			{
				await next(context1);
			}
			else
			{
				context1.Response.Redirect("/Ban");
			}
		}
		else
		{
			await next(context1);
		}
	}
}

public static class UserBanMiddlewareExtension
{
	public static IApplicationBuilder UseBanMiddleware(this IApplicationBuilder builder)
		=> builder.UseMiddleware<UserBanMiddleware>();
}