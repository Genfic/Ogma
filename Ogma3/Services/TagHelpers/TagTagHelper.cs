using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Utils.Extensions;

namespace Ogma3.Services.TagHelpers
{
    public class TagTagHelper : TagHelper
    {
        public string Color { get; set; }
        public string Href { get; set; }
        public int Opacity { get; set; } = 150;
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);

            if (Color != null)
            {
                var color = Color.ParseHexColor();
                var csColor = System.Drawing.Color.FromArgb(Opacity, color.R, color.G, color.B).ToCommaSeparatedCss();
                output.Attributes.Add("style", $"background:rgba({csColor})");
            }

            output.TagName = "a";
            output.AddClass("tag", NullHtmlEncoder.Default);
            output.Attributes.Add("href", Href);
            output.Content.SetHtmlContent(content.GetContent());
        }
    }
}