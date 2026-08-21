using System.Text.Encodings.Web;
using Markdig;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MinHash;
using Ogma3.Infrastructure.Constants;
using Utils.Extensions;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class MarkdownTagHelper : TagHelper
{
	public Presets Preset { get; set; } = Presets.Basic;

	[HtmlAttributeName("markdown")]
	public ModelExpression? MarkdownText { get; set; }

	/// <summary>
	/// If any h1 heading exists, all headings will be lowered by one level.
	/// </summary>
	[HtmlAttributeName("lower-headings")]
	public bool LowerHeadings { get; set; }

	[HtmlAttributeName(DictionaryAttributePrefix = "")]
	public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

	public enum Presets
	{
		Basic, // Default
		Comment,
		All,
		AllWithHtml,
		Blogpost,
	}

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		// Select preset
		var builder = Preset switch
		{
			Presets.Basic => MarkdownPipelines.Basic,
			Presets.Comment => MarkdownPipelines.Comment,
			Presets.All => MarkdownPipelines.All,
			Presets.AllWithHtml => MarkdownPipelines.AllWithHtml,
			Presets.Blogpost => MarkdownPipelines.Blogpost,
			_ => throw new EnumOutOfRangeException<Presets>(Preset),
		};

		var content = MarkdownText?.Model?.ToString() ?? await ChildContent(output);

		var markdownHtmlContent = LowerHeadings
			? DoLowerHeadings(Markdown.Parse(content, builder), builder)
			: Markdown.ToHtml(content, builder);

		output.TagName = "div";

		foreach (var (key, value) in Attributes)
		{
			output.Attributes.Add(key, value);
		}
		output.AddClass("md", HtmlEncoder.Default);

		output.Content.SetHtmlContent(markdownHtmlContent);
	}

	public async Task<string> ChildContent(TagHelperOutput output)
	{
		var childContent = await output.GetChildContentAsync(false, NullHtmlEncoder.Default);
		return childContent.GetContent(NullHtmlEncoder.Default).RemoveLeadingWhiteSpace();
	}

	private static string DoLowerHeadings(MarkdownDocument document, MarkdownPipeline pipeline)
	{
		if (document.Descendants<HeadingBlock>().All(h => h.Level != 1))
		{
			return document.ToHtml(pipeline);
		}

		foreach (var heading in document.Descendants<HeadingBlock>())
		{
			heading.Level = Math.Min(heading.Level + 1, 6);
		}

		return document.ToHtml(pipeline);
	}
}
