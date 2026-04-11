using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Services.IconService;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class IconTagHelper(IconCollector collector) : TagHelper
{
	public required string Name { get; set; }
	public int Size { get; set; } = 24;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		if (string.IsNullOrEmpty(Name)) return;

		collector.RegisterIcon(Name);

		output.TagName = "svg";

		output.AddClass("icon", HtmlEncoder.Default);

		output.Content.SetHtmlContent($"""<use href="#icon:{Name}"></use>""");

		output.Attributes.Add("part", "icon");
		output.Attributes.Add("width", Size);
		output.Attributes.Add("height", Size);
		output.Attributes.Add("viewbox", $"0 0 {Size} {Size}");

	}
}