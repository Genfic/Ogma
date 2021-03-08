using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers
{
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
            output.Attributes.SetAttribute("style", "padding: .5rem;font-size: .9rem;letter-spacing: .05rem;box-shadow: 0 0 0 4px black;outline: 4px dashed yellow;");
            output.Content.SetContent(JsonSerializer.Serialize(Object, options));
        }
    }
}