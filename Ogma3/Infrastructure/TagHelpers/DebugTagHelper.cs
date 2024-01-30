using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public class DebugTagHelper : TagHelper
{
	public required object Object { get; set; }
	
	private static JsonSerializerOptions SerializerOptions => new()
	{
		WriteIndented = true,
	};

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "pre";
		output.Attributes.SetAttribute("class", "debug");
		output.Content.SetContent(JsonSerializer.Serialize(Object, SerializerOptions));
	}
}