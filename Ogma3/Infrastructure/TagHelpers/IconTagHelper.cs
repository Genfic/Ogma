using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class IconTagHelper : TagHelper
{
	public string Icon { get; set; } = "bug_report";
	public bool Svg { get; set; } = false;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		if (Svg) 
		{
			output.TagName = "svg";
			output.Attributes.Add("width", "24");
			output.Attributes.Add("height", "24");
			output.Content.SetHtmlContent($"<use href=\"/svg/spritesheet.svg#{Icon}\"></use>");
		} 
		else
		{
			output.TagName = "i";
			output.AddClass("material-icons-outlined", HtmlEncoder.Default);
			output.Content.SetHtmlContent(Icon);
		}
	}
}