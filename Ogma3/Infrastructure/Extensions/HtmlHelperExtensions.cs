using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ogma3.Infrastructure.Extensions;

public static class HtmlHelperExtensions
{
	extension(IHtmlHelper helper)
	{
		public DateTime ToUserTime(DateTimeOffset time)
		{
			var ctx = helper.ViewContext.HttpContext;

			var timezoneInfo = ctx.User.GetTimeZoneInfo();
			var converted = TimeZoneInfo.ConvertTimeFromUtc(time.UtcDateTime, timezoneInfo);

			return converted;
		}

		public DateTime UserTimeNow()
			=> helper.ToUserTime(DateTimeOffset.Now);
	}
}