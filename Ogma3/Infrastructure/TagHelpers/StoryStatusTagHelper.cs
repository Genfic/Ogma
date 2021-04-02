using System;
using System.Text.Encodings.Web;
using EllipticCurve;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data.Stories;

namespace Ogma3.Infrastructure.TagHelpers
{
    public class StoryStatusTagHelper : TagHelper
    {
        public EStoryStatus Status { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var (icon, name) = Status switch
            {
                EStoryStatus.InProgress => ("autorenew", "In progress"),
                EStoryStatus.Completed  => ("done", "Completed"),
                EStoryStatus.OnHiatus   => ("pause", "On hiatus"),
                EStoryStatus.Cancelled  => ("block", "Cancelled"),
                _ => throw new ArgumentOutOfRangeException(nameof(Status), "The enum is somehow out of range")
            };
            
            output.TagName = "div";
            
            output.AddClass("story-status", NullHtmlEncoder.Default);
            output.AddClass(Status.ToString().ToLower(), HtmlEncoder.Default);
            
            output.Content.SetHtmlContent("");
            output.Content.AppendHtml($"<i class='material-icons-outlined'>{icon}</i>");
            output.Content.AppendHtml($"<span class='name'>{name}</span>");
        }
    }
}