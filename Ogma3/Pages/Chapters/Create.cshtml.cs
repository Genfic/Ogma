using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters;

[Authorize]
public sealed class CreateModel(ApplicationDbContext context, NotificationsRepository notificationsRepo)
	: PageModel
{
	[BindProperty]
	public required PostData Input { get; set; } = new();
	public required GetData Story { get; set; }

	public sealed class GetData
	{
		public required long Id { get; init; }
		public required string Slug { get; init; }
		public required string Title { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long id)
	{
		var story = await context.Stories
			.Where(s => s.Id == id)
			.Where(s => s.AuthorId == User.GetNumericId())
			.Select(s => new GetData
			{
				Id = s.Id,
				Title = s.Title,
				Slug = s.Slug,
			})
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();

		Story = story;

		return Page();
	}

	public sealed class PostData
	{
		public string Title { get; init; } = "";
		public string Body { get; init; } = "";
		[Display(Name = "Start notes")] public string? StartNotes { get; init; }
		[Display(Name = "End notes")] public string? EndNotes { get; init; }
	}

	public sealed class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			RuleFor(s => s.Title)
				.NotEmpty()
				.Length(CTConfig.Chapter.MinTitleLength, CTConfig.Chapter.MaxTitleLength);
			RuleFor(c => c.Body)
				.NotEmpty()
				.Length(CTConfig.Chapter.MinBodyLength, CTConfig.Chapter.MaxBodyLength);
			RuleFor(c => c.StartNotes)
				.MaximumLength(CTConfig.Chapter.MaxNotesLength);
			RuleFor(c => c.EndNotes)
				.MaximumLength(CTConfig.Chapter.MaxNotesLength);
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (!ModelState.IsValid) return Page();

		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		// Get the story to insert a chapter into. Include user in the search to check ownership.
		var story = await context.Stories
			.Where(s => s.Id == id)
			.Where(s => s.AuthorId == uid)
			.Include(s => s.Chapters)
			.Include(s => s.Shelves.Where(x => x.TrackUpdates))
			.FirstOrDefaultAsync();

		// Back to index if the story is null or author isn't the logged in user
		if (story is null) return Page();

		// Get the order number of the latest chapter
		var latestChapter = story.Chapters
			.OrderByDescending(c => c.Order)
			.Select(c => c.Order)
			.FirstOrDefault();

		// Construct new chapter
		var chapter = new Chapter
		{
			Title = Input.Title.Trim(),
			Slug = Input.Title.Trim().Friendlify(),
			Body = Input.Body.Trim(),
			StartNotes = Input.StartNotes?.Trim(),
			EndNotes = Input.EndNotes?.Trim(),
			Order = latestChapter + 1,
			CommentThread = new CommentThread(),
			WordCount = Input.Body.Words(),
		};

		// Recalculate words and chapters in the story
		story.WordCount = story.Chapters.Sum(c => c.WordCount) + chapter.WordCount;
		story.ChapterCount = story.Chapters.Count + 1;

		// Create the chapter and add it to the story
		story.Chapters.Add(chapter);

		// Subscribe author to the comment thread
		context.CommentThreadSubscribers.Add(new CommentThreadSubscriber
		{
			CommentThread = chapter.CommentThread,
			OgmaUserId = (long)uid,
		});

		await context.SaveChangesAsync();

		// Notify
		await notificationsRepo.Create(ENotificationEvent.WatchedStoryUpdated,
			story.Shelves.Select(s => s.OwnerId),
			"/Chapter",
			new { chapter.Id, chapter.Slug });

		return Routes.Pages.Chapter.Get(story.Id, chapter.Id, chapter.Slug).Redirect(this);
	}
}