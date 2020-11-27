using Markdig;
using MarkdigExtensions.Hashtags;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.Spoiler;

namespace Ogma3.Infrastructure
{
    public static class MarkdownPipelines
    {
        private static MentionOptions MentionOptions => new("/user/", "_blank");
        private static HashtagOptions HashtagOptions => new("/blog?q=", "_blank");
    
        public static MarkdownPipeline Basic => new MarkdownPipelineBuilder()
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();

        public static MarkdownPipeline Comment => new MarkdownPipelineBuilder()
            .UseMentions(MentionOptions)
            .UseAutoLinks()
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();

        public static MarkdownPipeline All => new MarkdownPipelineBuilder()
            .UsePipeTables()
            .UseMentions(MentionOptions)
            .UseAdvancedExtensions()
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();

        public static MarkdownPipeline Blogpost => new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseMentions(MentionOptions)
            .UseHashtags(HashtagOptions)
            .UseAutoIdentifiers()
            .UseSpoilers()
            .Build();
    }
}