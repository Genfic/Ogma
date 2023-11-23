using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Infrastructure.Json;

namespace Ogma3.Infrastructure.TagHelpers;

public class DebugTagHelper : TagHelper
{
	public object Object { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "pre";
		output.Attributes.SetAttribute("class", "debug");
		output.Content.SetContent(JsonSerializer.Serialize(Object, SerializerOptions.Indented));
	}
}