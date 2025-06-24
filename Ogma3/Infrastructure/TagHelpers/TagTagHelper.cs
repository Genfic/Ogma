using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data.Tags;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class TagTagHelper(LinkGenerator generator) : TagHelper
{
	public required TagDto Tag { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var href = Routes.Pages.Tag.Get(Tag.Id, Tag.Slug).Path(generator);

		var color = Tag.NamespaceColor?.Trim('#') ?? "transparent";

		output.TagName = "a";
		output.AddClass("tag", NullHtmlEncoder.Default);
		output.Attributes.Add("style", $"--tag-bg: #{color}");

		output.Attributes.Add("href", href);

		if (Tag.Namespace is {} ns)
		{
			output.Attributes.Add("title", ns.ToStringFast());
		}

		output.Content.AppendHtml($"<span class='name'>{Tag.Name}</span>");
	}
}