using System;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Services.TagHelpers
{
    public class MarkdownTagHelper : TagHelper
    {
        public Presets Preset { get; set; }

        public enum Presets
        {
            Basic, // Default
            All,
        }
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            MarkdownPipeline? pipeline = null;

            switch (Preset)
            {
                case Presets.All:
                    pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                    break;
                case Presets.Basic:
                    break;
            }
            
            var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            var markdownHtmlContent = Markdown.ToHtml(RemoveLeadingWhiteSpace(childContent.GetContent(NullHtmlEncoder.Default)), pipeline);
            
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "md");
            output.Content.SetHtmlContent(markdownHtmlContent);
        }

        private static string RemoveLeadingWhiteSpace(string content)
        {
            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return string.Join(Environment.NewLine, lines.Select(s => s.TrimStart(' ', '\t')));
        }
    }
}