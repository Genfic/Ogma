using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Services.TagHelpers
{
    public class AvatarTagHelper : TagHelper
    {
        public string Src { get; set; }
        public int? Size { get; set; }
        
        public string? Alt { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var url = Src;
            if (Size != null)
            {
                url = $"{url}?s={Size}";
                output.Attributes.SetAttribute("style", $"width:{Size}px;height:{Size}px");
            }

            output.TagName = "img";
            output.Attributes.SetAttribute("class", "avatar");
            output.Attributes.SetAttribute("src", url);
            output.Attributes.SetAttribute("alt", Alt ?? "");
            output.Attributes.Remove(new TagHelperAttribute("title"));
        }
        
    }
}