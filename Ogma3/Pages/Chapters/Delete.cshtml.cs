using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Chapters;

[Authorize]
public sealed class DeleteModel(ApplicationDbContext context) : PageModel
{
	[BindProperty]
	public required GetData Chapter { get; set; }

	public sealed class GetData
	{
		public required long Id { get; init; }
		public required DateTimeOffset? PublishDate { get; init; }
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public required int WordCount { get; init; }
		public required int CommentsThreadCommentsCount { get; init; }
		public required string StoryTitle { get; init; }
		public required long StoryId { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long id)
	{
		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == User.GetNumericId())
			.Select(c => new GetData
			{
				Id = c.Id,
				Title = c.Title,
				Slug = c.Slug,
				StoryId = c.StoryId,
				StoryTitle = c.Story.Title,
				WordCount = c.WordCount,
				PublishDate = c.PublicationDate,
				CommentsThreadCommentsCount = c.CommentsThread.CommentsCount,
			})
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();
		
		Chapter = chapter;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		// Get chapter
		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == User.GetNumericId())
			.Include(c => c.Story)
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();

		// Recalculate words and chapters in the story
		chapter.Story.WordCount -= chapter.WordCount;
		chapter.Story.ChapterCount -= 1;

		context.Chapters.Remove(chapter);

		await context.SaveChangesAsync();

		return RedirectToPage("../Story", new { id = chapter.StoryId });
	}
}