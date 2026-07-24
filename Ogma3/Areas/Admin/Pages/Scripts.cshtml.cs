using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.TagCache;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed class Scripts(ApplicationDbContext ctx, TagCache tagCache, OgmaConfig config) : PageModel
{
	public void OnGet()
	{

	}

	public string? Message { get; set; }

	public async Task<IActionResult> OnGetRecalculateBlogpostCutoff()
	{
		var posts = await ctx.Blogposts
			.Select(b => new
			{
				b.Id,
				Body = b.Body.Substring(0, config.BlogpostExcerptDefaultCutoff),
			})
			.ToListAsync();

		await using var transaction = await ctx.Database.BeginTransactionAsync();
		try
		{
			var rows = 0;
			foreach (var post in posts)
			{
				var cutoff = post.Body.LastIndexOf(' ');
				cutoff = cutoff > 0 ? cutoff : config.BlogpostExcerptDefaultCutoff;

				rows += await ctx.Blogposts
					.Where(b => b.Id == post.Id)
					.ExecuteUpdateAsync(s => s.SetProperty(b => b.ExcerptCutoff, cutoff));
			}
			await transaction.CommitAsync();

			Message = $"Updated excerpt cutoffs for {rows} blogposts.";
		}
		catch (Exception e)
		{
			Message = e.Message;
		}

		return Page();
	}

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