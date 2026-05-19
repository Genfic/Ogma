using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Reports;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class ReportsCountBadgeTagHelper(ApplicationDbContext ctx) : TagHelper
{
	[ViewContext]
	[HtmlAttributeNotBound]
	public required ViewContext ViewContext { get; init; }

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		var isStaff = ViewContext.HttpContext.User.IsStaff();
		if (!isStaff)
		{
			output.SuppressOutput();
			return;
		}

		var count = await ctx.Reports
			.Where(r => r.Status == ReportStatus.Open)
			.CountAsync();

		output.TagName = "span";
		output.Attributes.Add("class", "badge");
		output.Content.SetHtmlContent(count.ToString());
	}
}