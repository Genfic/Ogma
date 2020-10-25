#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Ogma3.Data.DTOs;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class TagTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;

        public TagTagHelper(IHttpContextAccessor accessor, LinkGenerator generator)
        {
            _accessor = accessor;
            _generator = generator;
            Tag = new TagDto();
        }

        public TagDto Tag { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var href = _generator
                           .GetUriByPage(_accessor.HttpContext, "/Tag", null, new { id = Tag.Id, slug = Tag.Slug });
            
            output.TagName = "a";
            output.AddClass("tag", NullHtmlEncoder.Default);
            
            output.Attributes.Add("href", href);
            output.Attributes.Add("title", Tag.NamespaceName);

            output.Content.AppendHtml(Tag.NamespaceColor == null
                ? "<div class='bg'></div>"
                : $@"<div class='bg' style='background-color: #{Tag.NamespaceColor.Trim('#')}'></div>");
            
            output.Content.AppendHtml($@"<span class='name'>{Tag.Name}</span>");
            
        }
    }
}