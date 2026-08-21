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

	public static MarkdownPipeline Basic { get; } = GetBase()
		.DisableHtml()
		.DisableHeadings()
		.UseAutoIdentifiers()
		.UseCenter()
		.Build();

	public static MarkdownPipeline Comment { get; } = GetBase()
		.DisableHtml()
		.DisableHeadings()
		.UseMentions(MentionOptions)
		.UseAutoLinks()
		.Build();

	public static MarkdownPipeline All { get; } = GetBase()
		.DisableHtml()
		.UseMentions(MentionOptions)
		.UseCustomAdvancedExtensions()
		.UseAutoIdentifiers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();

	public static MarkdownPipeline AllWithHtml { get; } = GetBase()
		.UseMentions(MentionOptions)
		.UseCustomAdvancedExtensions()
		.UseAutoIdentifiers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();

	public static MarkdownPipeline Blogpost { get; } = GetBase()
		.DisableHtml()
		.UseCustomAdvancedExtensions()
		.UseMentions(MentionOptions)
		.UseHashtags(HashtagOptions)
		.UseAutoIdentifiers()
		.UseCenter()
		.UsePollEmbeds()
		.Build();

	private static MarkdownPipelineBuilder GetBase() => new MarkdownPipelineBuilder()
		.UseHtmlComments()
		.UseEmphasisExtras()
		.UseSpoilers();

	private static MarkdownPipelineBuilder UseCustomAdvancedExtensions(this MarkdownPipelineBuilder builder)
		=> builder
			.UseAlertBlocks()
			.UseAbbreviations()
			.UseAutoIdentifiers()
			.UseCitations()
			.UseCustomContainers()
			.UseDefinitionLists()
			.UseFigures()
			.UseFooters()
			.UseFootnotes()
			.UseGridTables()
			.UseMathematics()
			.UseMediaLinks()
			.UseListExtras()
			.UseTaskLists()
			.UseDiagrams()
			.UseAutoLinks()
			.UseGenericAttributes();
}