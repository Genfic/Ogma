using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Controllers;

[Route("[controller]")]
public class ChapterController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public ChapterController(ApplicationDbContext context) => _context = context;

	// GET
	[HttpGet("/story/{sid:long}/chapter/first")]
	public async Task<IActionResult> FirstChapter(long sid)
	{
		var chapterData = await _context.Chapters
			.Where(ch => ch.StoryId == sid)
			.Where(ch => ch.PublicationDate != null)
			.Where(ch => ch.ContentBlockId == null)
			.OrderBy(ch => ch.Order)
			.Select(ch => new { ch.Id, ch.Slug })
			.FirstOrDefaultAsync();

		return RedirectToPage("/Chapter", new{ sid, chapterData.Id, chapterData.Slug });
	}

	// GET
	[HttpGet("/story/{sid:long}/chapter/last")]
	public async Task<IActionResult> LastChapter(long sid)
	{
		var chapterData = await _context.Chapters
			.Where(ch => ch.StoryId == sid)
			.Where(ch => ch.PublicationDate != null)
			.Where(ch => ch.ContentBlockId == null)
			.OrderByDescending(ch => ch.Order)
			.Select(ch => new { ch.Id, ch.Slug })
			.FirstOrDefaultAsync();

		return RedirectToPage("/Chapter", new{ sid, chapterData.Id, chapterData.Slug });
	}
}