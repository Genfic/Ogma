using System.Net;
using System.Net.Http.Json;
using Ogma3.Data.Quotes;

namespace Ogma3.Tests.Integration.Quotes;

[ClassDataSource<OgmaApiFactory>(Shared = SharedType.Globally)]
public sealed class GetAllQuotesTests(OgmaApiFactory factory)
{
    [Test]
    public async Task GetAllQuotes_Returns200_WhenDatabaseIsEmpty()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/quotes");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetAllQuotes_ReturnsEmptyList_WhenDatabaseIsEmpty()
    {
        var client = factory.CreateClient();

        var quotes = await client.GetFromJsonAsync<List<FullQuoteDto>>("/api/quotes");

        await Assert.That(quotes).IsNotNull();
        await Assert.That(quotes!).IsEmpty();
    }

    [Test]
    public async Task GetAllQuotes_ReturnsAllSeededQuotes()
    {
        // Seed directly via DbContext
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Ogma3.Data.ApplicationDbContext>();
        db.Quotes.AddRange(
            new Ogma3.Data.Quotes.Quote { Body = "Test quote 1", Author = "Author A" },
            new Ogma3.Data.Quotes.Quote { Body = "Test quote 2", Author = "Author B" }
        );
        await db.SaveChangesAsync();

        var client = factory.CreateClient();
        var quotes = await client.GetFromJsonAsync<List<FullQuoteDto>>("/api/quotes");

        await Assert.That(quotes).IsNotNull();
        await Assert.That(quotes!.Count).IsGreaterThanOrEqualTo(2);

        // Clean up so other tests start from a known state
        db.Quotes.RemoveRange(db.Quotes);
        await db.SaveChangesAsync();
    }
}
