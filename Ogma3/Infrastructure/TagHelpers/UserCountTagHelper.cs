using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;

namespace Ogma3.Infrastructure.TagHelpers;

public class UserCountTagHelper(ApplicationDbContext context, IMemoryCache cache) : TagHelper
{
	/// <summary>
	/// How often should the cache refresh in minutes
	/// </summary>
	public int CacheTime { get; set; } = 60;

	public override async Task ProcessAsync(TagHelperContext context1, TagHelperOutput output)
	{
		const string name = nameof(UserCountTagHelper) + "_cache";

		int count;
		if (cache.TryGetValue(name, out int c))
		{
			count = c;
		}
		else
		{
			count = await context.Users.CountAsync();
			cache.Set(name, count, TimeSpan.FromMinutes(CacheTime));
		}

		output.TagName = "span";
		output.Content.SetContent(count.ToString());
	}
}