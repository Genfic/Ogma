using System.Net;
using System.Net.Http.Json;
using Ogma3.Api.V1.Quotes;
using Ogma3.Data.Quotes;

namespace Ogma3.Tests.Integration.Quotes;

[ClassDataSource<OgmaApiFactory>(Shared = SharedType.Globally)]
public sealed class CreateQuoteTests(OgmaApiFactory factory)
{
    [Test]
    public async Task CreateQuote_Returns401_WhenNotAuthenticated()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/quotes",
            new CreateQuote.Command { Body = "Hello", Author = "World" });

        // Unauthenticated requests are redirected to /login by the cookie middleware
        await Assert.That((int)response.StatusCode)
            .IsGreaterThanOrEqualTo(300)
            .And.IsLessThan(500);
    }

    [Test]
    public async Task CreateQuote_Returns403_WhenAuthenticatedAsNonAdmin()
    {
        var identity = new System.Security.Claims.ClaimsIdentity(
            [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "99"),
             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "RegularUser")],
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme
        );
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory, identity);

        var response = await client.PostAsJsonAsync("/api/quotes",
            new CreateQuote.Command { Body = "Hello", Author = "World" });

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task CreateQuote_Returns201_WhenAdmin()
    {
        var client = await AuthHelper.CreateAdminClientAsync(factory);

        var response = await client.PostAsJsonAsync("/api/quotes",
            new CreateQuote.Command { Body = "Admin quote", Author = "Admin" });

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<FullQuoteDto>();
        await Assert.That(created).IsNotNull();
        await Assert.That(created!.Body).IsEqualTo("Admin quote");
        await Assert.That(created.Author).IsEqualTo("Admin");
        await Assert.That(created.Id).IsGreaterThan(0L);
    }

    [Test]
    public async Task CreateQuote_Returns422_WhenBodyIsEmpty()
    {
        var client = await AuthHelper.CreateAdminClientAsync(factory);

        var response = await client.PostAsJsonAsync("/api/quotes",
            new CreateQuote.Command { Body = "", Author = "Author" });

        // Immediate.Validations returns 422 Unprocessable Entity on validation failure
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task CreateQuote_Returns422_WhenAuthorIsEmpty()
    {
        var client = await AuthHelper.CreateAdminClientAsync(factory);

        var response = await client.PostAsJsonAsync("/api/quotes",
            new CreateQuote.Command { Body = "Some body", Author = "" });

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}
