using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MinHash;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.ChapterService;
using Routes.Pages;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters;

[Authorize]
public sealed class EditModel
(
	AppDbContext context,
	MinHasher hasher,
	ChapterService chapterService,
	ILogger<EditModel> logger) : PageModel
{
	[BindProperty]
	public required PostData Input { get; set; }

	public required bool WasPublished { get; set; }

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
				Publish = c.IsVisible,
				Schedule = c.ScheduledFor,
				StoryId = c.StoryId,
			})
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();

		Input = chapter;

		return Page();
	}

	public sealed class PostData
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required string Body { get; init; }
		[Display(Name = "Start notes")]
		public required string? StartNotes { get; init; }
		[Display(Name = "End notes")]
		public required string? EndNotes { get; init; }
		public required bool Publish { get; init; }
		public required long? StoryId { get; init; }
		public required DateTimeOffset? Schedule { get; init; }
	}

	public sealed class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			var now = DateTimeOffset.UtcNow;

			RuleFor(b => b.Title)
				.NotEmpty()
				.Length(CTConfig.Chapter.MinTitleLength, CTConfig.Chapter.MaxTitleLength);
			RuleFor(b => b.Body)
				.NotEmpty()
				.Length(CTConfig.Chapter.MinBodyLength, CTConfig.Chapter.MaxBodyLength);
			RuleFor(c => c.StartNotes)
				.MaximumLength(CTConfig.Chapter.MaxNotesLength);
			RuleFor(c => c.EndNotes)
				.MaximumLength(CTConfig.Chapter.MaxNotesLength);
			RuleFor(c => c.Publish)
				.NotNull();
			RuleFor(b => b.Schedule)
				.InclusiveBetween(now + CTConfig.Publication.MinDelay, now + CTConfig.Publication.MaxDelay)
				.When(b => b.Schedule is not null);
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		var signature = hasher.ComputeSignature(Input.Body.Trim());
		var copies = await chapterService.IsPlagiarized(signature, id);

		if (copies.Count > 0)
		{
			logger.LogWarning("User {UserId} tried to create a chapter on story {StoryId} that was similar to {@Copies}", uid, id, copies);
			ModelState.AddModelError("Body", "This chapter seems plagiarized.");
		}

		if (!ModelState.IsValid) return Page();

		var schedule = Input.Schedule is {} sch
			? TimeZoneInfo.ConvertTime(sch, User.GetTimeZoneInfo()).ToUniversalTime()
			: (DateTimeOffset?)null;

		var chapterEditRows = await context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == uid)
			.ExecuteUpdateAsync(spc => spc
				.SetProperty(c => c.Title, Input.Title.Trim())
				.SetProperty(c => c.Slug, Input.Title.Trim().Friendlify())
				.SetProperty(c => c.Body, Input.Body.Trim())
				.SetProperty(c => c.StartNotes, Input.StartNotes?.Trim())
				.SetProperty(c => c.EndNotes, Input.EndNotes?.Trim())
				.SetProperty(c => c.WordCount, Input.Body.Words())
				.SetProperty(c => c.IsVisible, Input.Publish)
				.SetProperty(c => c.Signature, hasher.ComputeSignature(Input.Body.Trim()))
				.If(Input.Publish, usb => usb
					.SetProperty(b => b.PublicationDate, b => b.PublicationDate ?? DateTimeOffset.UtcNow))
				.If(schedule is not null, usb => usb
					.SetProperty(b => b.ScheduledFor, schedule))
			);

		if (chapterEditRows <= 0) return NotFound("Chapter not found");

		var storyEditRows = await context.Stories
			.Where(s => s.AuthorId == uid)
			.Where(s => s.Chapters.Any(c => c.Id == id))
			.Select(s => new
			{
				Story = s,
				ChapterCount = s.Chapters.Count(c => c.IsVisible),
				WordCount = s.Chapters.Where(c => c.IsVisible).Sum(c => c.WordCount),
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

		return Chapter.Get(data.StoryId, data.Id, data.Slug).Redirect(this);
	}
}