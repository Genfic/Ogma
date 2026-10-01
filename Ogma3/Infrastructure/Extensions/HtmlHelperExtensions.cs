using Microsoft.AspNetCore.Mvc.Rendering;
using Ogma3.Data;

namespace Ogma3.Infrastructure.Extensions;

public static class HtmlHelperExtensions
{
	extension(IHtmlHelper helper)
	{
		public DateTime ToUserTime(DateTimeOffset time)
			=> helper.ViewContext.HttpContext.User.ToUserTime(time).DateTime;

		public DateTime UserTimeNow()
			=> helper.ToUserTime(DateTimeOffset.Now);

		public (string Min, string Max) ScheduleBounds()
		{
			var now = helper.UserTimeNow();
			now = now.AddSeconds(-now.Second);

			return (
				(now + CTConfig.Publication.MinDelay).ToString("s"),
				(now + CTConfig.Publication.MaxDelay).ToString("s")
			);
		}
	}
}