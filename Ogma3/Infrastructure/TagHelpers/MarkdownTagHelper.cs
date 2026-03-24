using System.ComponentModel;
using System.Text.Encodings.Web;
using Markdig;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Infrastructure.Constants;
using Utils.Extensions;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class MarkdownTagHelper : TagHelper
{
	public Presets Preset { get; set; } = Presets.Basic;

	[HtmlAttributeName(DictionaryAttributePrefix = "")]
	public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

	public enum Presets
	{
		Basic, // Default
		Comment,
		All,
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
			Presets.Blogpost => MarkdownPipelines.Blogpost,
			_ => throw new InvalidEnumArgumentException("Somehow the value passed to the enum param was not that enum..."),
		};

		var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
		var markdownHtmlContent = Markdown.ToHtml(childContent.GetContent(NullHtmlEncoder.Default).RemoveLeadingWhiteSpace(), builder);

		output.TagName = "div";

		foreach (var (key, value) in Attributes)
		{
			output.Attributes.Add(key, value);
		}
		output.AddClass("md", HtmlEncoder.Default);

		output.Content.SetHtmlContent(markdownHtmlContent);
	}
}