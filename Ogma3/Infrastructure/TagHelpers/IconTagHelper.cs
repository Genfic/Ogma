using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class IconTagHelper : TagHelper
{
	public string Icon { get; set; } = "bug_report";
	public bool FromSpritesheet { get; set; } = false;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "svg";

		output.AddClass("icon", HtmlEncoder.Default);
		output.Attributes.Add("part", "icon");
		output.Attributes.Add("width", "24");
		output.Attributes.Add("height", "24");
		output.Attributes.Add("viewbox", "0 0 24 24");
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