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

namespace Ogma3.Services.Middleware
{
    public class UserBanMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        
        public UserBanMiddleware(IMemoryCache cache, RequestDelegate next)
        {
            _cache = cache;
            _next = next;
        }
        
        public static string CacheKey(long id) => $"u{id}_Ban";
        
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var uid = context.User.GetNumericId();
            if (uid is null)
            {
                await _next(context);
                return;
            }

            var banDate = await _cache.GetOrCreateAsync(CacheKey((long)uid), async entry =>
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
                await _next(context);
                return;
            }

            if (banDate > DateTime.Now)
            {
                Log.Information("Banned user {Uid} tried accessing the site", uid);
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync($"Account banned until {banDate:o}");
                }
                else if (context.Request.Path.StartsWithSegments("/Ban"))
                {
                    await _next(context);
                }
                else
                {
                    context.Response.Redirect("/Ban");
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
    
    public static class UserBanMiddlewareExtension
    {
        public static IApplicationBuilder UseBanMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserBanMiddleware>();
        }
    }
}