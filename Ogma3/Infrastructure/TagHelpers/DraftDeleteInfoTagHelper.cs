using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Data.Subscriptions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.EntitlementService;
using System.Text.Encodings.Web;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class DraftDeleteInfoTagHelper(
	EntitlementService entitlementService,
	IHttpContextAccessor httpContextAccessor,
	OgmaConfig.OgmaConfig config
	) : TagHelper
{
	public required DateTimeOffset CreatedAt { get; set; }

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		if (httpContextAccessor.HttpContext?.User is not {} user || user.GetNumericId() is not {} uid)
		{
			output.SuppressOutput();
			return;
		}

		string msg;
		var entitlements = await entitlementService.GetEntitlements(uid);

		var tz = user.GetTimeZoneInfo() ?? TimeZoneInfo.Utc;

		if (entitlements is not {} ent)
		{
			msg = GetMessage(config.DraftRetentionDays, tz);
		}
		else if (ent.HasFlagFast(Entitlement.DraftsLastLonger))
		{
			msg = GetMessage(config.PremiumDraftRetentionDays, tz);
		}
		else if (ent.HasFlagFast(Entitlement.DraftsLastForever))
		{
			output.SuppressOutput();
			return;
		}
		else
		{
			msg = GetMessage(config.DraftRetentionDays, tz);
		}

		output.TagName = "div";
		output.AddClass("draft=delete-info", HtmlEncoder.Default);

		output.Content.SetHtmlContent(msg);
	}

	private string GetMessage(int days, TimeZoneInfo timeZone)
	{
		var expiry = CreatedAt + TimeSpan.FromDays(days);
		var expiryLocal = expiry.ToOffset(timeZone.BaseUtcOffset);

		return $"""This draft will be deleted <time datetime="{expiryLocal:yyyy-MM-ddTHH:mm:ssZ}">{expiryLocal:yyyy-MM-dd HH:mm:ss}</time>""";
	}
}