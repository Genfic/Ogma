using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Exceptions;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class StoryStatusTagHelper : TagHelper
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
			EStoryStatus.Unspecified => throw new UnexpectedEnumValueException<EStoryStatus>(Status, nameof(Status)),
			_ => throw new UnexpectedEnumValueException<EStoryStatus>(Status, nameof(Status)),
		};

		output.TagName = "div";

		output.AddClass("story-status", NullHtmlEncoder.Default);
		output.AddClass(Status.ToString().ToLower(), HtmlEncoder.Default);

		output.Content.SetHtmlContent("");
		output.Content.AppendHtml($"""
           <svg class="icon" height="24" width="24" part="icon">
               <use href="/svg/spritesheet.svg#{icon}"></use>
           </svg>
           """);
		output.Content.AppendHtml($"<span class='name'>{name}</span>");
	}
}