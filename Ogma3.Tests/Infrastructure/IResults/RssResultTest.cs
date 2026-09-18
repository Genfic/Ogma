using System.ServiceModel.Syndication;
using System.Text;
using Microsoft.AspNetCore.Http;
using Ogma3.Infrastructure.IResults;

namespace Ogma3.Tests.Infrastructure.IResults;

public sealed class RssResultTest
{
	[Test]
	public async Task TestExecuteAsyncWritesRssFeed()
	{
		await using var body = new MemoryStream();
		var context = new DefaultHttpContext
		{
			Response =
			{
				Body = body,
			},
		};

		var result = new RssResult(
			"Omega feed",
			"Description of the feed",
			[new SyndicationItem("An item", "Its content", new Uri("https://example.com/item"))],
			"https://example.com"
		);

		await result.ExecuteAsync(context);

		await Assert.That(context.Response.ContentType).IsEqualTo("application/rss+xml");

		body.Position = 0;
		using var reader = new StreamReader(body, Encoding.UTF8);
		var xml = await reader.ReadToEndAsync();
		await Assert.That(xml).Contains("<rss");
		await Assert.That(xml).Contains("Omega feed");
		await Assert.That(xml).Contains("An item");
	}
}