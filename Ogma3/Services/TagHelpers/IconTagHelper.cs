using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Services.TagHelpers
{
    public class IconTagHelper : TagHelper
    {
        public string Icon { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "i";
            output.AddClass("material-icons", HtmlEncoder.Default);
            output.Content.SetHtmlContent(Icon);
        }
    }
}