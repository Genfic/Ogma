using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Pages.Chapters;

public sealed class Locked(ApplicationDbContext context) : PageModel
{
	public required long StoryId { get; set; }
	public required long ChapterId { get; set; }
	public required string? Slug { get; set; }

	public required string? StorySlug { get; set; }


	public async Task OnGet(long sid, long id, string? slug)
	{
		StoryId = sid;
		ChapterId = id;
		Slug = slug;
		StorySlug = await context.Stories.Where(s => s.Id == sid).Select(s => s.Slug).FirstOrDefaultAsync();
	}
}