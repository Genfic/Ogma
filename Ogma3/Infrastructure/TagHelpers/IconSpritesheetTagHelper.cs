using System.Diagnostics;
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
			output.SuppressOutput();
			return;
		}

		var symbols = await cache.GetIcons(collector.RequestedIcons);

		output.TagName = "svg";
		output.Attributes.Add("xmlns", "http://www.w3.org/2000/svg");
		output.Attributes.Add("style", "display: none;");
		output.Attributes.Add("id", "icon-spritesheet");

		output.Content.AppendHtml("<defs>");
		foreach (var symbol in symbols)
		{
			output.Content.AppendFormat("""<symbol id="icon:{0}" viewBox="0 0 {1} {2}">""",
				symbol.Name,
				symbol.Width,
				symbol.Height
			);
			output.Content.AppendHtml(symbol.Body);
			output.Content.AppendHtml("</symbol>");

		}
		output.Content.AppendHtml("</defs>");

		output.Content.AppendFormat("<!-- generated {0} of {1} sprites in {2} ms -->",
			symbols.Count,
			collector.RequestedIcons.Count,
			start.ElapsedMilliseconds
		);
	}
}