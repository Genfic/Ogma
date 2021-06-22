using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Infrastructure.Attributes;

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
            output.AddClass("autoform", NullHtmlEncoder.Default);
            output.Attributes.Add("id", "config");
            output.Attributes.Add("method", "post");

            var obj = For.Model;
            var objName = For.Name;

            var groupedProps = obj
                .GetType()
                .GetProperties()
                .GroupBy(p => p.GetCustomAttribute<AutoformCategoryAttribute>()?.Name ?? "General")
                .ToDictionary(p => p.Key, p => p.ToList());

            foreach (var (key, propertyInfos) in groupedProps)
            {
                output.Content.AppendHtml("<details>");
                output.Content.AppendHtml($"<summary>{key}</summary>");
                
                foreach (var prop in propertyInfos)
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
                
                output.Content.AppendHtml("</details>");
            }
            
            output.Content.AppendHtml("<div class='o-form-group'>");
            output.Content.AppendHtml("<button type='submit' class='btn btn-primary'>Save</button>");
            output.Content.AppendHtml("</div>");

            var xcsrf = Generator.GenerateAntiforgery(ViewContext);
            if (xcsrf is not null)
            {
                output.PostContent.AppendHtml(xcsrf);
            }
        }
    }
}