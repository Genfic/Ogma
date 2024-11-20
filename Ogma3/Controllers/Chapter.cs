using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Controllers;

[Route("[controller]")]
[ExcludeFromDescription]
public sealed class ChapterController(ApplicationDbContext context) : ControllerBase
{
	// GET
	[HttpGet("/story/{sid:long}/chapter/first")]
	public async Task<IActionResult> FirstChapter(long sid)
	{
		var chapterData = await context.Chapters
			.Where(ch => ch.StoryId == sid)
			.Where(ch => ch.PublicationDate != null)
			.Where(ch => ch.ContentBlockId == null)
			.OrderBy(ch => ch.Order)
			.Select(ch => new { ch.Id, ch.Slug })
			.FirstOrDefaultAsync();
        
        if (chapterData is null) return NotFound();

		return Routes.Pages.Chapter.Get(sid, chapterData.Id, chapterData.Slug).Redirect(this);
	}

	// GET
	[HttpGet("/story/{sid:long}/chapter/last")]
	public async Task<IActionResult> LastChapter(long sid)
	{
		var chapterData = await context.Chapters
			.Where(ch => ch.StoryId == sid)
			.Where(ch => ch.PublicationDate != null)
			.Where(ch => ch.ContentBlockId == null)
			.OrderByDescending(ch => ch.Order)
			.Select(ch => new { ch.Id, ch.Slug })
			.FirstOrDefaultAsync();
        
        if (chapterData is null) return NotFound();

		return RedirectToPage("/Chapter", new{ sid, chapterData.Id, chapterData.Slug });
	}
}