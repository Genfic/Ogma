using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Ogma3.Tests.Integration;

/// <summary>
/// Registers a minimal <c>/test-auth/sign-in</c> endpoint on the TestServer that
/// converts a posted claims list into a real authentication cookie. This avoids
/// needing a working login UI or OAuth flow in tests.
/// </summary>
public static class TestAuthExtensions
{
    public record ClaimDto(string Type, string Value);

    /// <summary>
    /// Call this inside <c>WebApplicationFactory.ConfigureWebHost</c> >
    /// <c>Configure</c> to mount the sign-in helper endpoint.
    /// </summary>
    public static IEndpointRouteBuilder MapTestAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/test-auth/sign-in", async (HttpContext ctx, List<ClaimDto> claims) =>
        {
            var identity = new ClaimsIdentity(
                claims.Select(c => new Claim(c.Type, c.Value)),
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            var principal = new ClaimsPrincipal(identity);

            await ctx.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false }
            );

            return Results.Ok();
        });

        return app;
    }
}
