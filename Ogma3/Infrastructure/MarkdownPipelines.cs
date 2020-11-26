using Markdig;
using MarkdigExtensions.Hashtags;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.Spoiler;

namespace Ogma3.Infrastructure
{
    public static class MarkdownPipelines
    {
        public static MarkdownPipeline Basic => new MarkdownPipelineBuilder()
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();

        public static MarkdownPipeline Comment => new MarkdownPipelineBuilder()
            .UseMentions(new MentionOptions("/user/", "_blank"))
            .UseAutoLinks()
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();

        public static MarkdownPipeline All => new MarkdownPipelineBuilder()
            .UsePipeTables()
            .UseMentions(new MentionOptions("/user/", "_blank"))
            .UseAdvancedExtensions()
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();

        public static MarkdownPipeline Blogpost => new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseMentions(new MentionOptions("/user/", "_blank"))
            .UseHashtags(new HashtagOptions("/blog?q=", "_blank"))
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();
    }
}