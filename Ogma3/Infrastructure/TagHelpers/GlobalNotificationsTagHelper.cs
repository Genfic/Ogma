using System.Text;
using System.Text.Encodings.Web;
using Markdig;
using MemoryPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Constants;
using Sqids;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class GlobalNotificationsTagHelper(ApplicationDbContext ctx, IFusionCache cache, SqidsEncoder<long> sqids) : TagHelper
{
	[ViewContext]
	[HtmlAttributeNotBound]
	public required ViewContext ViewContext { get; set; }

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		var notifs = await cache.GetOrSetAsync("global-notifications", async ct => await GetData(ct));

		if (notifs.Count <= 0)
		{
			output.SuppressOutput();
			return;
		}

		var dismissed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		if (ViewContext.HttpContext.Request.Cookies.TryGetValue(CTConfig.Cookies.DismissedNotifications, out var cookie))
		{
			var span = cookie.AsSpan();
			foreach (var r in span.Split(','))
			{
				var id = span[r].Trim();
				if (!id.IsEmpty)
				{
					dismissed.Add(id.ToString());
				}
			}
		}

		var sb = new StringBuilder();
		foreach (var notif in notifs)
		{
			if (notif.Sqid is null || notif.Sqid is {} sq && dismissed.Contains(sq))
			{
				continue;
			}

			var item = /*lang=html*/
				$"""
				 <li global-notif-id={notif.Sqid} style="--color: #{notif.Color}">
				 	{notif.Message}
				 	<button class="close">&#10006;</button>
				 </li>
				 """;
			sb.Append(item);
		}

		if (sb.Length <= 0)
		{
			output.SuppressOutput();
			return;
		}

		output.TagName = "ul";
		output.AddClass("global-notifications", HtmlEncoder.Default);
		output.Content.SetHtmlContent(sb.ToString());
	}

	private async Task<IReadOnlyList<NotifDto>> GetData(CancellationToken ct)
	{
		var notifs = await ctx.GlobalNotifications
			.Where(n => n.ArchivedAt == null)
			.OrderByDescending(n => n.CreatedAt)
			.Select(n => new NotifDto(n.Id, n.Message, n.Color))
			.ToListAsync(ct);

		foreach (var notif in notifs)
		{
			notif.Message = Markdown.ToHtml(notif.Message, MarkdownPipelines.Basic);
			notif.Sqid = sqids.Encode(notif.Id);
		}

		return notifs;
	}
}

[MemoryPackable]
public sealed partial class NotifDto(long id, string message, string? color)
{
	public long Id { get; } = id;
	public string? Sqid { get; set; }
	public string Message { get; set; } = message;
	public string? Color { get; } = color;
}