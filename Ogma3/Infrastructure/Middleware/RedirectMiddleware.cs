#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ogma3.Infrastructure.Middleware;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RedirectMiddlewareOptions _options;
    
    public RedirectMiddleware(RequestDelegate next, IOptions<RedirectMiddlewareOptions> options)
    {
        _next = next;
        _options = options.Value;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.Redirects.TryGetValue(context.Request.Path, out var redirect))
        {
            Log.Information("Redirecting from {Source} to {Target}", context.Request.Path, redirect);
            context.Response.Redirect(redirect);
            return;
        }

        await _next(context);
    }
}

public static class RedirectMiddlewareExtensions
{
    public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder builder, Action<RedirectMiddlewareOptions> config)
    {
        var options = new RedirectMiddlewareOptions();
        config(options);
        return builder.UseMiddleware<RedirectMiddleware>(Options.Create(options));
    }
}

public class RedirectMiddlewareOptions
{
    public Dictionary<string, string> Redirects { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}