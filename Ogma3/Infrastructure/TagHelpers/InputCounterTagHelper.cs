using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Services;
using Serilog;
using Serilog.Core;
using Utils.Extensions;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class InputCounterTagHelper : TagHelper
    {
        public IHtmlHelper Context { get; set; }
        public string Label { get; set; }
        
        public string? Description { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var name = Label.Replace(" ", "");
            
            output.TagName = "div";
            output.AddClass("o-form-group", NullHtmlEncoder.Default);

            output.Content.AppendHtml($@"<label for=""{name}"">{Label}</label>");
            if (Description is not null)
            {
                output.Content.AppendHtml($@"<p class=""desc"">{Description}</p>");
            }
            output.Content.AppendHtml($@"<input id=""{name}"" name=""{name}"" type=""text"" class=""o-form-control active-border""/>");
            

            var r = Context.Resource(_ => new HelperResult(y => y.WriteAsync("Testing testing 123")), "css");
            Log.Debug(r.Value);
        }
    }
}