using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Infrastructure.TagHelpers;

[HtmlTargetElement("time-display")]
public sealed class TimeDisplayTagHelper : TagHelper
{
	[ViewContext]
	[HtmlAttributeNotBound]
	public required ViewContext ViewContext { get; init; }

	public required DateTimeOffset UtcDate { get; set; }

	public Func<DateTime, string>? Formatter { get; set; }

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "time";
		output.Attributes.SetAttribute("datetime", UtcDate.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture));

		var timezone = ViewContext.HttpContext.User.TryGetClaim(ClaimTypes.Timezone, out var tz) ? tz : "UTC";

		var timezoneInfo = TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out var tzi) ? tzi : TimeZoneInfo.Utc;
		var converted = TimeZoneInfo.ConvertTimeFromUtc(UtcDate.UtcDateTime, timezoneInfo);

		if (!output.Attributes.ContainsName("title"))
		{
			output.Attributes.SetAttribute("title", converted.ToString("dd MMM yyyy HH:mm", CultureInfo.InvariantCulture));
		}

		if (Formatter is not null)
		{
			var formatted = Formatter(converted);

			output.Content.SetContent(formatted);
		}
		else
		{
			var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);
			if (content.IsEmptyOrWhiteSpace)
			{
				output.Content.SetContent(UtcDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
			}
			else
			{
				output.Content.SetHtmlContent(content);
			}
		}
	}
}