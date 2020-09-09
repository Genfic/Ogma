using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Utils.Extensions;

namespace Ogma3.Services.TagHelpers
{
    public class TagTagHelper : TagHelper
    {
        public string Color { get; set; }
        public string Href { get; set; }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            
            output.TagName = "a";
            output.AddClass("tag", NullHtmlEncoder.Default);
            output.Attributes.Add("href", Href);

            output.Content.AppendHtml(Color != null
                ? $@"<div class='bg' style='background-color: #{Color.Trim('#')}'></div>"
                : @"<div class='bg'></div>");

            output.Content.AppendHtml($@"<span class='name'>{content.GetContent()}</span>");
            
        }
    }
}