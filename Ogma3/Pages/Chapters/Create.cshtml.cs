using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MinHash;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Notifications;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters;

[Authorize]
public sealed class CreateModel(ApplicationDbContext context, NotificationsRepository notificationsRepo, MinHasher hasher)
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

		var storyDto = await context.Stories
			.Where(s => s.Id == id && s.AuthorId == uid)
			.Select(s => new
			{
				s.Id,
				s.Title,
				LatestChapterOrder = s.Chapters
					.OrderByDescending(c => c.Order)
					.Select(c => c.Order)
					.FirstOrDefault(),
				ChapterCount = s.Chapters.Count(),
				CurrentWordCount = s.Chapters.Sum(c => c.WordCount),
				ShelfOwnerIds = s.Shelves
					.Where(x => x.TrackUpdates)
					.Select(x => x.OwnerId)
					.ToList(),
			})
			.FirstOrDefaultAsync();

		if (storyDto is null) return Page();

		var chapter = new Chapter
		{
			Title = Input.Title.Trim(),
			Slug = Input.Title.Trim().Friendlify(),
			Body = Input.Body.Trim(),
			StartNotes = Input.StartNotes?.Trim(),
			EndNotes = Input.EndNotes?.Trim(),
			Order = storyDto.LatestChapterOrder + 1,
			StoryId = storyDto.Id,
			CommentThread = new CommentThread(),
			WordCount = Input.Body.Words(),
			Signature = hasher.ComputeSignature(Input.Body.Trim()),
		};

		var newWordCount = storyDto.CurrentWordCount + chapter.WordCount;
		var newChapterCount = storyDto.ChapterCount + 1;

		context.Chapters.Add(chapter);

		context.CommentThreadSubscribers.Add(new CommentThreadSubscriber
		{
			CommentThread = chapter.CommentThread,
			OgmaUserId = (long)uid,
		});

		await context.SaveChangesAsync();

		await context.Stories
			.Where(s => s.Id == storyDto.Id)
			.ExecuteUpdateAsync(s => s
				.SetProperty(x => x.WordCount, newWordCount)
				.SetProperty(x => x.ChapterCount, newChapterCount));

		await notificationsRepo.Create(
			ENotificationEvent.WatchedStoryUpdated,
			storyDto.ShelfOwnerIds,
			Routes.Pages.Chapter.Get(storyDto.Id, chapter.Id, chapter.Slug).Url(Url) ?? "",
			$"A new chapter was added to {storyDto.Title}");

		return Routes.Pages.Chapter.Get(storyDto.Id, chapter.Id, chapter.Slug).Redirect(this);
	}
}