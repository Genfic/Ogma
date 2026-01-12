using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Riok.Mapperly.Abstractions;
using Routes.Pages;
using Chapter = Ogma3.Data.Chapters.Chapter;

namespace Ogma3.Pages;

public sealed class ChapterModel(ApplicationDbContext context) : PageModel
{
	public required ChapterDetails Chapter { get; set; }
	public required ChapterMicroDto? Previous { get; set; }
	public required ChapterMicroDto? Next { get; set; }
	public required CommentsThreadDto CommentsThread { get; set; }

	public async Task<IActionResult> OnGetAsync(long sid, long id, string? slug)
	{
		var uid = User.GetNumericId();

		// If the user is not logged in, check if the story is locked to logged-in users only.
		if (uid is null)
		{
			var locked = await context.Stories.Where(s => s.Id == sid).Select(s => s.IsLocked).FirstOrDefaultAsync();
			if (locked)
			{
				return Chapters_Locked.Get(sid, id, slug).Redirect(this);
			}
		}

		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.PublicationDate != null || c.Story.AuthorId == uid)
			.Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid || User.IsStaff())
			.ProjectToDetails()
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();
		Chapter = chapter;

		CommentsThread = new CommentsThreadDto
		{
			Id = chapter.CommentThreadId,
			Type = CommentSource.Chapter,
			LockDate = chapter.CommentThreadLockDate,
		};

		var chapterQuery = context.Chapters
			.Where(c => c.StoryId == Chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.Where(c => c.ContentBlockId == null);

		Previous = await chapterQuery
			.Where(c => c.Order < Chapter.Order)
			.OrderByDescending(c => c.Order)
			.ProjectToMicro()
			.FirstOrDefaultAsync();
		Next = await chapterQuery
			.Where(c => c.Order > Chapter.Order)
			.OrderBy(c => c.Order)
			.ProjectToMicro()
			.FirstOrDefaultAsync();

		return Page();
	}
}

public sealed record ChapterDetails
(
	long Id,
	string Title,
	string Slug,
	uint Order,
	string Body,
	string? StartNotes,
	string? EndNotes,
	string StoryRatingName,
	DateTimeOffset? PublicationDate,
	long StoryId,
	string StoryTitle,
	string StorySlug,
	long StoryAuthorId,
	long CommentThreadId,
	DateTimeOffset? CommentThreadLockDate,
	ContentBlockCard? ContentBlock);

public sealed record ChapterMicroDto(long Id, string Title, string Slug);

[Mapper]
public static partial class ChapterMapper
{
	public static partial IQueryable<ChapterMicroDto> ProjectToMicro(this IQueryable<Chapter> c);
	public static partial IQueryable<ChapterDetails> ProjectToDetails(this IQueryable<Chapter> c);
}