#nullable enable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data.Tags;
using Utils.Extensions;

namespace Ogma3.Infrastructure.TagHelpers;

public class TagTagHelper : TagHelper
{
    private readonly IUrlHelper _urlHelper;

    public TagTagHelper(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
    {
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        Tag = new TagDto();
    }

    public TagDto Tag { get; set; }
        
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var href = _urlHelper.Page("/Tag", new { id = Tag.Id, slug = Tag.Slug });
            
        output.TagName = "a";
        output.AddClass("tag", NullHtmlEncoder.Default);
            
        output.Attributes.Add("href", href);
            
        if (Tag.Namespace is not null)
        {
            output.Attributes.Add("title", Tag.Namespace.GetAttribute<DisplayAttribute>()?.Name ?? Tag.Namespace.ToString());
        }

        output.Content.AppendHtml(Tag.NamespaceColor == null
            ? "<div class='bg'></div>"
            : $@"<div class='bg' style='background-color: #{Tag.NamespaceColor.Trim('#')}'></div>");
            
        output.Content.AppendHtml($@"<span class='name'>{Tag.Name}</span>");
            
    }
}