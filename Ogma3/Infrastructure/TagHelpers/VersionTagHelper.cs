using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public class VersionTagHelper : TagHelper
{
	public string Text { get; set; }
	public string Color { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "span";
		output.Content.SetContent(Text);
		output.Attributes.SetAttribute("id", "version");
		output.Attributes.Add("style", $"background: {Color}");
	}
}