using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.TagCache;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed class Scripts(ApplicationDbContext ctx, TagCache tagCache) : PageModel
{
	public void OnGet()
	{

	}

	public string? Message { get; set; }

	public async Task<IActionResult> OnGetReloadTagCache()
	{
		var tags = await ctx.Tags
			.Select(t => new TagEntry(t.Id, t.Name, t.Namespace))
			.ToListAsync();

		var entries = await tagCache.AddManyAsync(tags);

		Message = $"Fetched {tags.Count} tags and created {entries} cache entries.";

		return Page();
	}

	public async Task<IActionResult> OnGetUpdateStories()
	{
		var sc = await ctx.Stories.ExecuteUpdateAsync(setters => setters
			.SetProperty(s => s.IsVisible, s => s.PublicationDate != null));

		var bc = await ctx.Blogposts.ExecuteUpdateAsync(setters => setters
			.SetProperty(s => s.IsVisible, s => s.PublicationDate != null));

		var cc = await ctx.Chapters.ExecuteUpdateAsync(setters => setters
			.SetProperty(s => s.IsVisible, s => s.PublicationDate != null));

		Message = $"{sc} stories updated, {bc} blogposts updated, {cc} chapters updated";

		return Page();
	}
}