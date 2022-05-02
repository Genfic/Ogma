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

public class UserBanMiddleware : IMiddleware
{
	private readonly IMemoryCache _cache;
	private readonly ApplicationDbContext _context;

	public UserBanMiddleware(IMemoryCache cache, ApplicationDbContext context)
	{
		_cache = cache;
		_context = context;
	}

	public static string CacheKey(long id) => $"u{id}_Ban";

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var uid = context.User.GetNumericId();
		if (uid is null)
		{
			await next(context);
			return;
		}

		var banDate = await _cache.GetOrCreateAsync(CacheKey((long)uid), async entry =>
		{
			entry.SlidingExpiration = TimeSpan.FromMinutes(30);
			return await _context.Infractions
				.Where(i => i.UserId == uid)
				.Where(i => i.Type == InfractionType.Ban)
				.Where(i => i.RemovedAt == null)
				.OrderByDescending(i => i.ActiveUntil)
				.Select(i => i.ActiveUntil)
				.FirstOrDefaultAsync();
		});

		if (banDate == default)
		{
			await next(context);
			return;
		}

		if (banDate > DateTime.Now)
		{
			Log.Information("Banned user {UserId} tried accessing the site", uid);
			if (context.Request.Path.StartsWithSegments("/api"))
			{
				context.Response.Clear();
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				await context.Response.WriteAsync($"Account banned until {banDate:o}");
			}
			else if (context.Request.Path.StartsWithSegments("/Ban"))
			{
				await next(context);
			}
			else
			{
				context.Response.Redirect("/Ban");
			}
		}
		else
		{
			await next(context);
		}
	}
}

public static class UserBanMiddlewareExtension
{
	public static IApplicationBuilder UseBanMiddleware(this IApplicationBuilder builder)
		=> builder.UseMiddleware<UserBanMiddleware>();
}