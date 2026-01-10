using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class IconTagHelper : TagHelper
{
	public string Icon { get; set; } = "bug_report";
	public bool FromSpritesheet { get; set; } = false;
	public int Size { get; set; } = 24;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "svg";

		output.AddClass("icon", HtmlEncoder.Default);
		output.Attributes.Add("part", "icon");
		output.Attributes.Add("width", Size);
		output.Attributes.Add("height", Size);
		output.Attributes.Add("viewbox", $"0 0 {Size} {Size}");

		if (FromSpritesheet)
		{
			output.Content.SetHtmlContent($"""<use href="/svg/spritesheet.svg#{Icon}"></use>""");
		}
		else
		{
			output.Content.SetHtmlContent(Icons.GetSvg(Icon));
		}
	}
}