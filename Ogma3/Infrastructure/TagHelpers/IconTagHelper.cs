using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public class IconTagHelper : TagHelper
{
	public string Icon { get; set; } = "bug_report";

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "i";
		output.AddClass("material-icons-outlined", HtmlEncoder.Default);
		output.Content.SetHtmlContent(Icon);
	}
}