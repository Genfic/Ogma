using Markdig;
using MarkdigExtensions.Center;
using MarkdigExtensions.Hashtags;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.PollEmbed;
using MarkdigExtensions.Spoiler;

namespace Ogma3.Infrastructure.Constants;

public static class MarkdownPipelines
{
	private static MentionOptions MentionOptions { get; } = new("/user/", "_blank");
	private static HashtagOptions HashtagOptions { get; } = new("/blog?q=", "_blank");

	public static MarkdownPipeline Basic { get; } = new MarkdownPipelineBuilder()
		.DisableHtml()
		.DisableHeadings()
		.UseAutoIdentifiers()
		.UseEmphasisExtras()
		.UseSpoilers()
		.UseCenter()
		.Build();

	public static MarkdownPipeline Comment { get; } = new MarkdownPipelineBuilder()
		.DisableHtml()
		.DisableHeadings()
		.UseMentions(MentionOptions)
		.UseAutoLinks()
		.UseEmphasisExtras()
		.UseSpoilers()
		.Build();

	public static MarkdownPipeline All { get; } = new MarkdownPipelineBuilder()
		.DisableHtml()
		.UsePipeTables()
		.UseEmphasisExtras()
		.UseMentions(MentionOptions)
		.UseAdvancedExtensions()
		.UseAutoIdentifiers()
		.UseSpoilers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();

	public static MarkdownPipeline AllWithHtml { get; } = new MarkdownPipelineBuilder()
		.UsePipeTables()
		.UseEmphasisExtras()
		.UseMentions(MentionOptions)
		.UseAdvancedExtensions()
		.UseAutoIdentifiers()
		.UseSpoilers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();

	public static MarkdownPipeline Blogpost { get; } = new MarkdownPipelineBuilder()
		.DisableHtml()
		.UseAdvancedExtensions()
		.UseEmphasisExtras()
		.UseMentions(MentionOptions)
		.UseHashtags(HashtagOptions)
		.UseAutoIdentifiers()
		.UseSpoilers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();
}