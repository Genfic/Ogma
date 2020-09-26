using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class VersionTagHelper : TagHelper
    {
        private string Text { get; set; }
        private string Color { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Content.SetContent(Text);
            output.Attributes.SetAttribute("id", "version");
            output.Attributes.Add("style", $"background: {Color}");
        }

    }
}