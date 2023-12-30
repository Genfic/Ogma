using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Utils.Extensions;

namespace Ogma3.Infrastructure.Formatters;

public class RssOutputFormatter : TextOutputFormatter
{
	private readonly IConfiguration _config;

	public RssOutputFormatter(IConfiguration config)
	{
		_config = config;
		SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/rss+xml"));
		SupportedEncodings.AddMany(Encoding.UTF8, Encoding.UTF32);
	}

	protected override bool CanWriteType(Type? type)
	{
		return typeof(RssResult).IsAssignableFrom(type) ||
		       typeof(IEnumerable<RssResult>).IsAssignableFrom(type);
	}

	public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
	{
		var now = DateTime.Now;
		
		var domain = $"https://{_config.GetValue<string>("Domain")}";

		var data = (RssResult?)context.Object;
		if (data is null) throw new ArgumentException("Passed object was not of type RssResult");

		var feed = new SyndicationFeed(
			data.Title,
			data.Description,
			new Uri(domain),
			"RssUrl",
			now
		)
		{
			Copyright = new TextSyndicationContent($"2019 — {now.Year}"),
			Items = data.Items
		};

		using var stream = new MemoryStream();
		await using var writer = XmlWriter.Create(stream, new XmlWriterSettings
		{
			Encoding = Encoding.UTF8,
			NewLineHandling = NewLineHandling.Entitize,
			NewLineOnAttributes = true,
			Indent = true,
			Async = true
		});
		new Rss20FeedFormatter(feed, false).WriteTo(writer);
		await writer.FlushAsync();
		stream.Seek(0, SeekOrigin.Begin);

		var result = new byte[stream.Length];
		_ = await stream.ReadAsync(result.AsMemory(0, (int)stream.Length));

		await context.HttpContext.Response.WriteAsync(selectedEncoding.GetString(result), selectedEncoding);
	}
}

public record RssResult
{
	public required string Title { get; init; }
	public required string Description { get; init; }
	public required IEnumerable<SyndicationItem> Items { get; init; }
}