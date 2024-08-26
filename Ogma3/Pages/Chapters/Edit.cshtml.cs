using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters;

[Authorize]
public class EditModel(ApplicationDbContext context) : PageModel
{
	[BindProperty]
	public required PostData Input { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		// Get chapter
		var chapter = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == User.GetNumericId())
			.Select(c => new PostData
			{
				Id = c.Id,
				Title = c.Title,
				Body = c.Body,
				StartNotes = c.StartNotes,
				EndNotes = c.EndNotes,
				IsPublished = c.PublicationDate != null,
				StoryId = c.StoryId,
			})
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();

		Input = chapter;

		return Page();
	}

	public class PostData
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required string Body { get; init; }
		[Display(Name = "Start notes")]
		public required string? StartNotes { get; init; }
		[Display(Name = "End notes")]
		public required string? EndNotes { get; init; }
		public required bool IsPublished { get; init; }
		public required long? StoryId { get; init; }
	}

	public class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			RuleFor(b => b.Title)
				.NotEmpty()
				.Length(CTConfig.CChapter.MinTitleLength, CTConfig.CChapter.MaxTitleLength);
			RuleFor(b => b.Body)
				.NotEmpty()
				.Length(CTConfig.CChapter.MinBodyLength, CTConfig.CChapter.MaxBodyLength);
			RuleFor(c => c.StartNotes)
				.MaximumLength(CTConfig.CChapter.MaxNotesLength);
			RuleFor(c => c.EndNotes)
				.MaximumLength(CTConfig.CChapter.MaxNotesLength);
			RuleFor(c => c.IsPublished)
				.NotNull();
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		if (!ModelState.IsValid) return Page();

		var chapterEditRows = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == uid)
			.ExecuteUpdateAsync(spc => spc
				.SetProperty(c => c.Title, Input.Title.Trim())
				.SetProperty(c => c.Slug, Input.Title.Trim().Friendlify())
				.SetProperty(c => c.Body, Input.Body.Trim())
				.SetProperty(c => c.StartNotes, Input.StartNotes == null ? null : Input.StartNotes.Trim())
				.SetProperty(c => c.EndNotes, Input.EndNotes == null ? null : Input.EndNotes.Trim())
				.SetProperty(c => c.WordCount, Input.Body.Words())
				.SetProperty(c => c.PublicationDate, Input.IsPublished ? DateTime.Now : null)
			);

		if (chapterEditRows <= 0) return NotFound("Chapter not found");

		var storyEditRows = await context.Stories
			.Where(s => s.AuthorId == uid)
			.Where(s => s.Chapters.Any(c => c.Id == id))
			.Select(s => new
			{
				Story = s, 
				ChapterCount = s.Chapters.Count(c => c.PublicationDate != null),
				WordCount = s.Chapters.Where(c => c.PublicationDate != null).Sum(c => c.WordCount),
			})
			.ExecuteUpdateAsync(spc => spc
				.SetProperty(s => s.Story.WordCount, s => s.WordCount)
				.SetProperty(s => s.Story.ChapterCount, s => s.ChapterCount)
			);

		if (storyEditRows <= 0) return NotFound("Story not found");

		var data = await context.Chapters
			.Where(c => c.Id == id)
			.Select(c => new
			{
				c.Id,
				c.Slug,
				c.StoryId,
			})
			.FirstOrDefaultAsync();

		if (data is null) return NotFound();

		return RedirectToPage("../Chapter", new { sid = data.StoryId, id = data.Id, slug = data.Slug });
	}
}