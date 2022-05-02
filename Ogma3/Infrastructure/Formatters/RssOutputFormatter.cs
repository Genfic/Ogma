using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace Ogma3.Infrastructure.Formatters;

public class RssOutputFormatter : TextOutputFormatter
{
	private readonly IConfiguration _config;

	public RssOutputFormatter(IConfiguration config)
	{
		_config = config;
		SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/rss+xml"));
		SupportedEncodings.Add(Encoding.UTF8);
		SupportedEncodings.Add(Encoding.UTF32);
	}

	protected override bool CanWriteType(Type type)
	{
		return typeof(RssResult).IsAssignableFrom(type) ||
		       typeof(IEnumerable<RssResult>).IsAssignableFrom(type);
	}

	public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
	{
		var domain = $"https://{_config.GetValue<string>("Domain")}";

		var data = (RssResult)context.Object;
		if (data is null) throw new ArgumentException("Passed object was not of type RssResult");

		var feed = new SyndicationFeed(
			data.Title,
			data.Description,
			new Uri(domain),
			"RssUrl",
			DateTime.Now
		)
		{
			Copyright = new TextSyndicationContent($"2019 — {DateTime.Now.Year}"),
			Items = data.Items
		};

		var stream = new MemoryStream();
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

		var cd = new ContentDisposition
		{
			Inline = true,
			FileName = "rss"
		};
		context.HttpContext.Response.Headers.Add("Content-Disposition", cd.ToString());

		await context.HttpContext.Response.WriteAsync(selectedEncoding.GetString(result), selectedEncoding);
	}
}

public record RssResult
{
	public string Title { get; init; }
	public string Description { get; init; }
	public IEnumerable<SyndicationItem> Items { get; init; }
}