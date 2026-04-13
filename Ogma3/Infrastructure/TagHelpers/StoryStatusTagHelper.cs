using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Services.IconService;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class StoryStatusTagHelper(IconCollector collector) : TagHelper
{
	public EStoryStatus Status { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var (icon, name) = Status switch
		{
			EStoryStatus.InProgress => ("ic:round-autorenew", "In progress"),
			EStoryStatus.Completed => ("ic:round-check", "Completed"),
			EStoryStatus.OnHiatus => ("ic:round-pause", "On hiatus"),
			EStoryStatus.Cancelled => ("ic:round-block", "Cancelled"),
			_ => throw new UnexpectedEnumValueException<EStoryStatus>(Status, nameof(Status)),
		};

		collector.RegisterIcon(icon);

		output.TagName = "div";

		output.AddClass("story-status", NullHtmlEncoder.Default);
		output.AddClass("tag", NullHtmlEncoder.Default);
		output.AddClass(Status.ToStringFast().ToLower(), HtmlEncoder.Default);

		output.Content.SetHtmlContent("");
		output.Content.AppendHtml($"""
           <svg class="icon" viewBox="0 0 16 16" height="16px" width="16px" part="icon">
               <use href="#icon:{icon}"></use>
           </svg>
           """);
		output.Content.AppendHtml($"""<span class="name">{name}</span>""");
	}
}