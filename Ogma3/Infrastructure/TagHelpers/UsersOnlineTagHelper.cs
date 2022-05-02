using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;

namespace Ogma3.Infrastructure.TagHelpers;

/// <summary>
/// Get the cached amount of users online
/// </summary>
public class UsersOnlineTagHelper : TagHelper
{
	private readonly ApplicationDbContext _context;
	private readonly IMemoryCache _cache;

	public UsersOnlineTagHelper(ApplicationDbContext context, IMemoryCache cache)
	{
		_context = context;
		_cache = cache;
	}

	/// <summary>
	/// Tolerance in minutes
	/// </summary>
	public int Tolerance { get; set; } = 10;

	/// <summary>
	/// How often should the cache refresh in minutes
	/// </summary>
	public int CacheTime { get; set; } = 60;

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		const string name = nameof(UsersOnlineTagHelper) + "_cache";

		var count = await _cache.GetOrCreateAsync(name, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheTime);

			var minutesAgo = DateTime.UtcNow.AddMinutes(-Tolerance);
			return await _context.Users
				.TagWith("Getting currently-active users")
				.Where(u => u.LastActive >= minutesAgo)
				.CountAsync();
		});

		output.TagName = "span";
		output.Content.SetContent(count.ToString());
	}
}