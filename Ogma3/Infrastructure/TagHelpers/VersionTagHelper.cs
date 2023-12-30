using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public class VersionTagHelper : TagHelper
{
	public required string Text { get; set; }
	public required string Color { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "span";
		output.Content.SetContent(Text);
		output.Attributes.SetAttribute("id", "version");
		output.Attributes.Add("style", $"background: {Color}");
	}
}