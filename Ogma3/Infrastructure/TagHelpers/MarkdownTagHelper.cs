#nullable enable
using System.ComponentModel;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Infrastructure.Constants;
using Utils.Extensions;

namespace Ogma3.Infrastructure.TagHelpers;

public class MarkdownTagHelper : TagHelper
{
    public Presets Preset { get; set; }

    public string Class { get; set; } = "";

    public enum Presets
    {
        Basic, // Default
        Comment,
        All,
        Blogpost
    }
        
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Select preset
        var builder = Preset switch
        {
            Presets.Basic    => MarkdownPipelines.Basic,
            Presets.Comment  => MarkdownPipelines.Comment,
            Presets.All      => MarkdownPipelines.All,
            Presets.Blogpost => MarkdownPipelines.Blogpost,
            _ => throw new InvalidEnumArgumentException("Somehow the value passed to the enum param was not that enum...")
        };
            
        var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
        var markdownHtmlContent = Markdown.ToHtml(childContent.GetContent(NullHtmlEncoder.Default).RemoveLeadingWhiteSpace(), builder);
            
        output.TagName = "div";
        output.Attributes.SetAttribute("class", $"md {Class}");
        output.Content.SetHtmlContent(markdownHtmlContent);
    }
}