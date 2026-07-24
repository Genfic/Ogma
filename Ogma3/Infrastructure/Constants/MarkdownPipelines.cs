using Markdig;
using MarkdigExtensions.Center;
using MarkdigExtensions.Hashtags;
using MarkdigExtensions.HtmlComments;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.PollEmbed;
using MarkdigExtensions.Spoiler;

namespace Ogma3.Infrastructure.Constants;

public static class MarkdownPipelines
{
	private static MentionOptions MentionOptions { get; } = new("/user/", "_blank");
	private static HashtagOptions HashtagOptions { get; } = new("/blog?q=", "_blank");

	private static readonly MarkdownPipelineBuilder Base = new MarkdownPipelineBuilder()
		.UseHtmlComments()
		.UseEmphasisExtras()
		.UseSpoilers();

	public static MarkdownPipeline Basic { get; } = Base
		.DisableHtml()
		.DisableHeadings()
		.UseAutoIdentifiers()
		.UseCenter()
		.Build();

	public static MarkdownPipeline Comment { get; } = Base
		.DisableHtml()
		.DisableHeadings()
		.UseMentions(MentionOptions)
		.UseAutoLinks()
		.Build();

	public static MarkdownPipeline All { get; } = Base
		.DisableHtml()
		.UsePipeTables()
		.UseMentions(MentionOptions)
		.UseAdvancedExtensions()
		.UseAutoIdentifiers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();

	public static MarkdownPipeline AllWithHtml { get; } = Base
		.UsePipeTables()
		.UseMentions(MentionOptions)
		.UseAdvancedExtensions()
		.UseAutoIdentifiers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();

	public static MarkdownPipeline Blogpost { get; } = Base
		.DisableHtml()
		.UseAdvancedExtensions()
		.UseMentions(MentionOptions)
		.UseHashtags(HashtagOptions)
		.UseAutoIdentifiers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();
}