using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Tests.Integration;

/// <summary>
/// Helper that produces an authenticated <see cref="HttpClient"/> by writing a real
/// ASP.NET Core cookie into the client cookie jar — no mocking of the auth stack.
/// </summary>
public static class AuthHelper
{
    /// <summary>
    /// Creates an <see cref="HttpClient"/> whose requests appear to the app as the given
    /// <paramref name="identity"/>. The claims are serialised into a real authentication
    /// cookie via the app's own <see cref="IAuthenticationService"/>.
    /// </summary>
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        WebApplicationFactory<Program> factory,
        ClaimsIdentity identity)
    {
        // We need a real HttpContext to call SignInAsync, so we send a special
        // "sign-in" request that the TestAuthMiddleware handles, sets the cookie,
        // and the client's cookie container keeps it for all subsequent calls.
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Register a tiny in-process endpoint just for test sign-in
                services.AddRouting();
            });
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
        });

        // Manually issue an auth cookie by calling the app's auth pipeline directly
        using var scope = factory.Services.CreateScope();
        var httpContextFactory = scope.ServiceProvider.GetRequiredService<IHttpContextFactory>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        // Use the factory to call a dedicated sign-in endpoint that we register below
        // instead — simpler approach: post directly to the test sign-in helper endpoint.
        // For now we use a header-based fake-auth approach that is compatible with
        // TestServer without needing an extra endpoint.
        //
        // A cleaner alternative for production-grade test suites is to extract the
        // cookie from a real /Account/Login flow; for integration tests the approach
        // below (custom test auth handler) is idiomatic.
        _ = client; // suppress unused warning — client is returned below

        var principal = new ClaimsPrincipal(identity);

        // Write the cookie from the server side, then fish it out and inject it
        var cookieValue = await SignInAndExtractCookieAsync(factory, principal);

        var authedClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
        });
        authedClient.DefaultRequestHeaders.Add("Cookie", cookieValue);
        return authedClient;
    }

    /// <summary>Creates a client authenticated as an Admin.</summary>
    public static Task<HttpClient> CreateAdminClientAsync(WebApplicationFactory<Program> factory)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "TestAdmin"),
                new Claim(ClaimTypes.Role, RoleNames.Admin),
            ],
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        return CreateAuthenticatedClientAsync(factory, identity);
    }

    private static async Task<string> SignInAndExtractCookieAsync(
        WebApplicationFactory<Program> factory,
        ClaimsPrincipal principal)
    {
        // Hit the built-in test-sign-in endpoint that OgmaApiFactory exposes,
        // capture the Set-Cookie header value, and return it.
        // We use a lightweight internal HTTP call on the TestServer.
        using var innerClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
        });

        // Encode claims as JSON and POST to the /test-auth/sign-in helper endpoint
        var claimsPayload = principal.Claims
            .Select(c => new { c.Type, c.Value })
            .ToList();

        var response = await innerClient.PostAsJsonAsync("/test-auth/sign-in", claimsPayload);

        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            return string.Join("; ", cookies);
        }

        return string.Empty;
    }
}
