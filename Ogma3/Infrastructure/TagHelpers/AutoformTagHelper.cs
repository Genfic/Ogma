using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class AutoformTagHelper : TagHelper
    {        
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private IHtmlGenerator Generator { get; }
        
        public AutoformTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public ModelExpression For { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "form";
            output.AddClass("form", NullHtmlEncoder.Default);
            output.Attributes.Add("id", "config");
            output.Attributes.Add("method", "post");

            var obj = For.Model;
            var props = obj.GetType().GetProperties();
            var objName = For.Name;

            foreach (var prop in props)
            {
                var type = Type.GetTypeCode(prop.PropertyType) switch
                {
                    TypeCode.Int16    => "number",
                    TypeCode.Int32    => "number",
                    TypeCode.Int64    => "number",
                    TypeCode.UInt16   => "number",
                    TypeCode.UInt32   => "number",
                    TypeCode.UInt64   => "number",
                    TypeCode.Double   => "number",
                    TypeCode.Decimal  => "number",
                    TypeCode.Boolean  => "checkbox",
                    TypeCode.DateTime => "date",
                    _                 => "text"
                };
                var label = prop.Name;
                var value = prop.GetValue(obj);

                output.Content.AppendHtml("<div class='o-form-group'>");
                output.Content.AppendHtml($"<label for='{objName}_{label}'>{label}</label>");
                output.Content.AppendHtml($"<input type='{type}' id='{objName}_{label}' name='{objName}.{label}' value='{value}' class='o-form-control active-border'>");
                output.Content.AppendHtml("</div>");
            }
            
            output.Content.AppendHtml("<div class='o-form-group'>");
            output.Content.AppendHtml("<button type='submit' class='btn btn-primary'>Save</button>");
            output.Content.AppendHtml("</div>");

            var XCSRF = Generator.GenerateAntiforgery(ViewContext);
            if (XCSRF != null)
            {
                output.PostContent.AppendHtml(XCSRF);
            }
        }
    }
}