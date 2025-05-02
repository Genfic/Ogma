using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.IO;

namespace Ogma3.Infrastructure.IResults;

public sealed class RssResult(string title, string description, IEnumerable<SyndicationItem> items, string domain) : IResult
{
	private static readonly RecyclableMemoryStreamManager Manager = new();

	public async Task ExecuteAsync(HttpContext httpContext)
	{
		var now = DateTimeOffset.UtcNow;

		var feed = new SyndicationFeed(
			title,
			description,
			new Uri(domain),
			"RssUrl",
			now
		)
		{
			Copyright = new TextSyndicationContent($"2019 — {now.Year}"),
			Items = items,
		};

		await using var stream = Manager.GetStream();

		await using var writer = XmlWriter.Create(stream, new XmlWriterSettings
		{
			Encoding = Encoding.UTF8,
			NewLineHandling = NewLineHandling.Entitize,
			NewLineOnAttributes = true,
			Indent = true,
			Async = true,
		});

		new Rss20FeedFormatter(feed, false).WriteTo(writer);
		await writer.FlushAsync();

		httpContext.Response.ContentType = "application/rss+xml";

		stream.Position = 0;
		await stream.CopyToAsync(httpContext.Response.Body);
	}
}