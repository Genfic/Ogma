using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages;

public class ChapterModel(ApplicationDbContext context) : PageModel
{
	public required ChapterDetails Chapter { get; set; }

	public async Task<IActionResult> OnGetAsync(long sid, long id, string? slug)
	{
		var uid = User.GetNumericId();

		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.PublicationDate != null || c.Story.AuthorId == uid)
			.Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid || User.IsStaff())
			.Select(_mapChapterDetails)
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();
		Chapter = chapter;

		Chapter.Previous = await context.Chapters
			.Where(c => c.StoryId == Chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.Where(c => c.ContentBlockId == null)
			.Where(c => c.Order < Chapter.Order)
			.OrderBy(c => c.Order)
			.Select(_mapChapterMicro)
			.LastOrDefaultAsync();
		Chapter.Next = await context.Chapters
			.Where(c => c.StoryId == Chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.Where(c => c.ContentBlockId == null)
			.Where(c => c.Order > Chapter.Order)
			.OrderBy(c => c.Order)
			.Select(_mapChapterMicro)
			.FirstOrDefaultAsync();

		return Page();
	}

	public record ChapterDetails
	(
		long Id,
		string Title,
		string Slug,
		uint Order,
		string Body,
		string? StartNotes,
		string? EndNotes,
		string StoryRatingName,
		DateTime? PublicationDate,
		long StoryId,
		string StoryTitle,
		string StorySlug,
		long StoryAuthorId,
		CommentsThreadDto CommentsThread,
		ContentBlockCard? ContentBlock)
	{
		public ChapterMicroDto? Previous { get; set; }
		public ChapterMicroDto? Next { get; set; }
	}

	private static Expression<Func<Chapter, ChapterDetails>> _mapChapterDetails = c => new ChapterDetails(
		c.Id, c.Title, c.Slug, c.Order, c.Body,
		c.StartNotes, c.EndNotes,
		c.Story.Rating.Name,
		c.PublicationDate,
		c.StoryId, c.Story.Title, c.Story.Slug, c.Story.AuthorId,
		new CommentsThreadDto(c.CommentsThread.Id, nameof(Data.Chapters.Chapter), c.CommentsThread.LockDate),
		c.ContentBlock == null
			? null
			: new ContentBlockCard(c.ContentBlock.Reason, c.ContentBlock.DateTime, c.ContentBlock.Issuer.UserName)
	);

	public record ChapterMicroDto(long Id, string Title, string Slug, uint Order);

	private static Expression<Func<Chapter, ChapterMicroDto>> _mapChapterMicro = c => new ChapterMicroDto(c.Id, c.Title, c.Slug, c.Order);
}