using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ogma3.Services.Middleware;

public class RequestTimestampMiddleware
{
    private readonly RequestDelegate _next;
    public const string Name = "RequestStartedOn";

    public RequestTimestampMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        context.Items.Add(Name, DateTime.UtcNow);

        // Call the next delegate/middleware in the pipeline
        return _next(context);
    }
}
    
public static class RequestTimestampMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTimestamp(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTimestampMiddleware>();
    }
}