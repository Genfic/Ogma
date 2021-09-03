using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;
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
        
        public static string CacheKey(string name) => $"{name.ToUpperInvariant().Normalize()}Ban";
        
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var username = context.User.GetUsername()?.ToUpperInvariant()?.Normalize();
            if (username is null)
            {
                await _next(context);
                return;
            }

            var banDate = await _cache.GetOrCreateAsync(CacheKey(username), async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return await dbContext.Users
                    .Where(u => u.NormalizedUserName == username)
                    .Select(u => u.BannedUntil)
                    .FirstOrDefaultAsync();
            });

            if (banDate is null)
            {
                await _next(context);
                return;
            }

            if (banDate > DateTime.Now)
            {
                Log.Information("Banned user {Name} tried accessing the site", username);
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
                var user = await dbContext.Users
                    .Where(u => u.NormalizedUserName == username)
                    .FirstOrDefaultAsync();
                user.BannedUntil = null;
                await dbContext.SaveChangesAsync();
                    
                _cache.Set(CacheKey(username), user.BannedUntil);
                Log.Information("Banned user {Name} was automatically unbanned", username);
                    
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