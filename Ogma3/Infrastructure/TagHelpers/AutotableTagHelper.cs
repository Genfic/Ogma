using System;
using System.Collections;
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
    public class AutotableTagHelper: TagHelper
    {
        public ModelExpression For { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "table";

            var obj = For.Model;

            var props = obj
                .GetType()
                .GetProperties()
                .ToList();

            foreach (var p in props)
            {
                var value = p.GetValue(obj);
                var val = value switch
                {
                    IEnumerable and not string => JsonSerializer.Serialize(value),
                    _ => value
                };
                
                output.Content.AppendHtml("<tr>");
                output.Content.AppendHtml($"<td>{p.Name}</td>");
                output.Content.AppendHtml($"<td>{val}</td>");
                output.Content.AppendHtml("</tr>");
            }
        }
    }
}