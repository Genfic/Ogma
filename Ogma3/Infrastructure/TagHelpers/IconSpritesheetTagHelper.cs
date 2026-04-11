using System.Diagnostics;
using System.Text.Json;
using Cysharp.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Services.IconService;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class IconSpritesheetTagHelper(IconCache cache, IconCollector collector) : TagHelper
{
	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		var start = Stopwatch.StartNew();

		if (collector.RequestedIcons.Count <= 0)
		{
			return;
		}

		var symbols = await Task.WhenAll(collector.RequestedIcons.Select(async i => new
		{
			Name = i,
			Icon = await cache.GetIcon(i),
		}));

		using var sb = ZString.CreateStringBuilder(true);

		sb.AppendLine("<defs>");
		foreach (var symbol in symbols)
		{
			if (symbol is not { Icon: {} icon, Name: var name })
			{
				sb.AppendLine($"<!-- missing icon: {JsonSerializer.Serialize(symbol)} -->");
			}
			else
			{
				sb.AppendFormat("""<symbol id="icon:{0}" viewBox="0 0 {1} {2}" width="{1}px" height="{2}px">{3}</symbol>{4}""",
					name,
					icon.Width,
					icon.Height,
					icon.Body,
					'\n'
				);
			}

		}
		sb.Append("</defs>");

		output.TagName = "svg";
		output.Attributes.Add("xmlns", "http://www.w3.org/2000/svg");
		output.Attributes.Add("style", "display: none;");
		output.Content.SetHtmlContent(sb.ToString());
		output.Attributes.Add("data-gen-time", $"{start.ElapsedMilliseconds}ms");
	}
}