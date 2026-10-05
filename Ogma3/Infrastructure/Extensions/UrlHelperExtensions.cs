using Microsoft.AspNetCore.Mvc;

namespace Ogma3.Infrastructure.Extensions;

public static class UrlHelperExtensions
{
	extension(IUrlHelper urlHelper)
	{
		public string EnsureLocal(string? url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return urlHelper.Content("~/");
			}

			return urlHelper.IsLocalUrl(url)
				? url
				: urlHelper.Content("~/");
		}
	}
}