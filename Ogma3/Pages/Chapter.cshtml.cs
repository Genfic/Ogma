﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Pages;

public class ChapterModel(ApplicationDbContext context) : PageModel
{
	public required ChapterDetails Chapter { get; set; }
	public required ChapterMicroDto? Previous { get; set; }
	public required ChapterMicroDto? Next { get; set; }
	public required CommentsThreadDto CommentsThread { get; set; }

	public async Task<IActionResult> OnGetAsync(long sid, long id, string? slug)
	{
		var uid = User.GetNumericId();

		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.PublicationDate != null || c.Story.AuthorId == uid)
			.Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid || User.IsStaff())
			.ProjectToDetails()
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();
		Chapter = chapter;

		CommentsThread =
			new CommentsThreadDto(chapter.CommentsThreadId, nameof(Data.Chapters.Chapter), chapter.CommentsThreadLockDate);

		Previous = await context.Chapters
			.Where(c => c.StoryId == Chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.Where(c => c.ContentBlockId == null)
			.Where(c => c.Order < Chapter.Order)
			.OrderBy(c => c.Order)
			.ProjectToMicro()
			.LastOrDefaultAsync();
		Next = await context.Chapters
			.Where(c => c.StoryId == Chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.Where(c => c.ContentBlockId == null)
			.Where(c => c.Order > Chapter.Order)
			.OrderBy(c => c.Order)
			.ProjectToMicro()
			.FirstOrDefaultAsync();

		return Page();
	}
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
	long CommentsThreadId,
	DateTime CommentsThreadLockDate,
	ContentBlockCard? ContentBlock);

public record ChapterMicroDto(long Id, string Title, string Slug);

[Mapper]
public static partial class ChapterMapper
{
	public static partial IQueryable<ChapterMicroDto> ProjectToMicro(this IQueryable<Chapter> c);
	public static partial IQueryable<ChapterDetails> ProjectToDetails(this IQueryable<Chapter> c);
}