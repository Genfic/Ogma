using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;

namespace Ogma3.Infrastructure.TagHelpers;

public sealed class UserCountTagHelper(ApplicationDbContext dbContext, IMemoryCache cache) : TagHelper
{
	/// <summary>
	/// How often should the cache refresh in minutes
	/// </summary>
	public int CacheTime { get; set; } = 60;

	public override async Task ProcessAsync(TagHelperContext httpContext, TagHelperOutput output)
	{
		const string name = nameof(UserCountTagHelper) + "_cache";

		var count = await cache.GetOrCreateAsync(name, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheTime);
			return await dbContext.Users.TagWith("Getting current user count").CountAsync();
		});

		output.TagName = "span";
		output.Content.SetContent(count.ToString());
	}
}