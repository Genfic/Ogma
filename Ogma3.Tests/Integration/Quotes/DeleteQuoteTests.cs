using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Ogma3.Data;
using Ogma3.Data.Quotes;

namespace Ogma3.Tests.Integration.Quotes;

[ClassDataSource<OgmaApiFactory>(Shared = SharedType.Globally)]
public sealed class DeleteQuoteTests(OgmaApiFactory factory)
{
    private async Task<long> SeedQuoteAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var quote = new Quote { Body = "To be deleted", Author = "Doomed Author" };
        db.Quotes.Add(quote);
        await db.SaveChangesAsync();
        return quote.Id;
    }

    [Test]
    public async Task DeleteQuote_Returns401_WhenNotAuthenticated()
    {
        var id = await SeedQuoteAsync();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync($"/api/quotes/{id}");

        await Assert.That((int)response.StatusCode)
            .IsGreaterThanOrEqualTo(300)
            .And.IsLessThan(500);
    }

    [Test]
    public async Task DeleteQuote_Returns403_WhenNonAdmin()
    {
        var id = await SeedQuoteAsync();
        var identity = new System.Security.Claims.ClaimsIdentity(
            [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "99"),
             new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "RegularUser")],
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme
        );
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory, identity);

        var response = await client.DeleteAsync($"/api/quotes/{id}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task DeleteQuote_Returns200_WhenAdmin()
    {
        var id = await SeedQuoteAsync();
        var client = await AuthHelper.CreateAdminClientAsync(factory);

        var response = await client.DeleteAsync($"/api/quotes/{id}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task DeleteQuote_Returns404_WhenQuoteDoesNotExist()
    {
        var client = await AuthHelper.CreateAdminClientAsync(factory);

        var response = await client.DeleteAsync("/api/quotes/99999999");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }
}
