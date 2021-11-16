using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ogma3.Services;

public static class RoutingHelpers
{
    private const string ApiRoutePrefix = "/api";

    private static bool IsApiRequest(this HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(ApiRoutePrefix))
        {
            return true;
        }
		
        // Check if this is an ApiController
        var endpoint = context.GetEndpoint();

        return endpoint is not null && endpoint.Metadata.Any(o => o is ApiControllerAttribute);
    }
        
    public static Func<RedirectContext<CookieAuthenticationOptions>, Task> HandleApiRequest(int statusCode, Func<RedirectContext<CookieAuthenticationOptions>, Task> original)
    {
        return redirectContext =>
        {
            if (!redirectContext.HttpContext.IsApiRequest()) 
                return original(redirectContext);
                    
            redirectContext.Response.StatusCode = statusCode;
            return Task.CompletedTask;

        };
    }
}