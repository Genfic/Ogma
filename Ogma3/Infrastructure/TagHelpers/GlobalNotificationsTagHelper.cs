using System.Text;
using System.Text.Encodings.Web;
using Markdig;
using MemoryPack;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class GlobalNotificationsTagHelper(ApplicationDbContext ctx, IFusionCache cache) : TagHelper
{
	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		var notifs = await cache.GetOrSetAsync("global-notifications", await GetData());

		var sb = new StringBuilder();
		foreach (var notif in notifs)
		{
			sb.Append("""<li style="--color: #""");
			sb.Append(notif.Color);
			sb.Append('"');
			sb.Append('>');
			sb.Append(notif.Message);
			sb.AppendLine("</li>");
		}

		output.TagName = "ul";
		output.AddClass("global-notifications", HtmlEncoder.Default);
		output.Content.SetHtmlContent(sb.ToString().Trim());
	}

	private async Task<List<NotifDto>> GetData()
	{
		var notifs = await ctx.GlobalNotifications
			.Where(n => n.ArchivedAt == null)
			.OrderByDescending(n => n.CreatedAt)
			.Select(n => new NotifDto(n.Message, n.Color))
			.ToListAsync();

		for (var i = 0; i < notifs.Count; i++)
		{
			notifs[i] = notifs[i] with
			{
				Message = Markdown.ToHtml(notifs[i].Message, MarkdownPipelines.Basic),
			};
		}

		return notifs;
	}
}

[MemoryPackable]
public sealed partial record NotifDto(string Message, string? Color);