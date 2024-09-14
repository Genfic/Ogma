using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data.Tags;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class TagTagHelper : TagHelper
{
	private readonly IUrlHelper _urlHelper;

	public TagTagHelper(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
	{
		if (actionContextAccessor is not { ActionContext: not null })
			throw new NullReferenceException(nameof(actionContextAccessor.ActionContext));
		_urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
	}

	public required TagDto Tag { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var href = _urlHelper.Page("/Tag", new { id = Tag.Id, slug = Tag.Slug });

		output.TagName = "a";
		output.AddClass("tag", NullHtmlEncoder.Default);

		output.Attributes.Add("href", href);

		if (Tag.Namespace is {} ns)
		{
			output.Attributes.Add("title", ns.ToStringFast());
		}

		output.Content.AppendHtml(Tag.NamespaceColor is not {} color
			? "<div class='bg'></div>"
			: $"<div class='bg' style='background-color: #{color.Trim('#')}'></div>");

		output.Content.AppendHtml($"<span class='name'>{Tag.Name}</span>");
	}
}