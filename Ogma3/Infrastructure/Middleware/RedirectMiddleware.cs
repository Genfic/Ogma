using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Ogma3.Infrastructure.Middleware;

public class RedirectMiddleware : IMiddleware
{
    private readonly Dictionary<string, string> _redirects = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/.well-known/change-password"] = "/identity/account/manage/changepassword"
    }; 
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_redirects.TryGetValue(context.Request.Path, out var redirect))
        {
            Log.Information("Redirecting from {Source} to {Target}", context.Request.Path, redirect);
            context.Response.Redirect(redirect);
            return;
        }

        await next(context);
    }
}

public static class RedirectMiddlewareExtensions
{
    public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder builder) 
        => builder.UseMiddleware<RedirectMiddleware>();
}