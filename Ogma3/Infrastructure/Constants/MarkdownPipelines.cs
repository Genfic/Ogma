using Markdig;
using MarkdigExtensions.Hashtags;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.Spoiler;

namespace Ogma3.Infrastructure.Constants;

public static class MarkdownPipelines
{
	private static MentionOptions MentionOptions => new("/user/", "_blank");
	private static HashtagOptions HashtagOptions => new("/blog?q=", "_blank");

	public static MarkdownPipeline Basic => new MarkdownPipelineBuilder()
		.DisableHtml()
		.DisableHeadings()
		.UseAutoIdentifiers()
		.UseEmphasisExtras()
		.UseSpoilers()
		.Build();

	public static MarkdownPipeline Comment => new MarkdownPipelineBuilder()
		.DisableHtml()
		.DisableHeadings()
		.UseMentions(MentionOptions)
		.UseAutoLinks()
		.UseEmphasisExtras()
		.UseSpoilers()
		.Build();

	public static MarkdownPipeline All => new MarkdownPipelineBuilder()
		.DisableHtml()
		.UsePipeTables()
		.UseEmphasisExtras()
		.UseMentions(MentionOptions)
		.UseAdvancedExtensions()
		.UseAutoIdentifiers()
		.UseSpoilers()
		.Build();

	public static MarkdownPipeline Blogpost => new MarkdownPipelineBuilder()
		.DisableHtml()
		.UseAdvancedExtensions()
		.UseEmphasisExtras()
		.UseMentions(MentionOptions)
		.UseHashtags(HashtagOptions)
		.UseAutoIdentifiers()
		.UseSpoilers()
		.Build();
}