#nullable enable
using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Markdig;
using MarkdigExtensions.Hashtags;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.Spoiler;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Services.TagHelpers
{
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
            // Options
            var hashtagOptions = new HashtagOptions("/blog?q=", "_blank");
            var mentionOptions = new MentionOptions("/user/", "_blank");
            
            var builder = Preset switch
            {
                Presets.Basic   => new MarkdownPipelineBuilder(),
                
                Presets.Comment => new MarkdownPipelineBuilder()
                    .UseMentions(mentionOptions)
                    .UseAutoLinks(),
                
                Presets.All     => new MarkdownPipelineBuilder()
                    .UseMentions(mentionOptions)
                    .UseAdvancedExtensions(),
                
                Presets.Blogpost => new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UseMentions(mentionOptions)
                    .UseHashtags(hashtagOptions),
                
                _ => throw new InvalidEnumArgumentException("Somehow the value passed to the enum param was not that enum...")
            };

            // attach universal plugins and build the pipeline
            var pipeline = builder
                .UseSpoilers()
                .Build();
            
            var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            var markdownHtmlContent = Markdown.ToHtml(RemoveLeadingWhiteSpace(childContent.GetContent(NullHtmlEncoder.Default)), pipeline);
            
            output.TagName = "div";
            output.Attributes.SetAttribute("class", $"md {Class}");
            output.Content.SetHtmlContent(markdownHtmlContent);
        }

        private static string RemoveLeadingWhiteSpace(string content)
        {
            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return string.Join(Environment.NewLine, lines.Select(s => s.TrimStart(' ', '\t')));
        }
    }
}