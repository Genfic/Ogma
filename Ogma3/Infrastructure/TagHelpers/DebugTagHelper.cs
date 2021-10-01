using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers;

public class DebugTagHelper : TagHelper
{
    public object Object { get; set; }
        
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
            
        output.TagName = "pre";
        output.Attributes.SetAttribute("class", "debug");
        output.Content.SetContent(JsonSerializer.Serialize(Object, options));
    }
}