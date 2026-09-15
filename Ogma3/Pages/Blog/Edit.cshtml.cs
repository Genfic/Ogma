using System.ComponentModel;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Pages.Shared.Minimals;
using Routes.Pages;
using Utils.Extensions;

namespace Ogma3.Pages.Blog;

[Authorize]
public sealed class EditModel(AppDbContext context, OgmaConfig config) : PageModel
{
	[BindProperty]
	public required PostData Input { get; set; }

	public required bool WasPublished { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		// Get the logged-in user
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		// Get post and make sure the user matches
		var input = await context.Blogposts
			.Where(m => m.Id == id)
			.Where(b => b.AuthorId == uid)
			.Select(b => new PostData
			{
				Id = b.Id,
				Title = b.Title,
				Body = b.Body,
				Tags = string.Join(", ", b.Hashtags),
				Publish = b.IsVisible,
				PublicationDate = b.PublicationDate,
				Schedule = b.ScheduledFor,
				IsLocked = b.IsLocked,
				AttachedChapter = b.AttachedChapter == null ? null : new ChapterMinimal
				{
					Id = b.AttachedChapter.Id,
					Title = b.AttachedChapter.Title,
					Slug = b.AttachedChapter.Slug,
					PublicationDate = b.AttachedChapter.PublicationDate,
					IsVisible = b.AttachedChapter.IsVisible,
					StoryId = b.AttachedChapter.StoryId,
					StoryTitle = b.AttachedChapter.Story.Title,
					StoryAuthorUserName = b.AttachedChapter.Story.Author.UserName,
				},
				AttachedStory = b.AttachedStory == null ? null : new StoryMinimal
				{
					Id = b.AttachedStory.Id,
					Title = b.AttachedStory.Title,
					Slug = b.AttachedStory.Slug,
					PublicationDate = b.AttachedStory.PublicationDate,
					IsVisible = b.AttachedStory.IsVisible,
					AuthorUserName = b.AttachedStory.Author.UserName,
				},
			})
			.FirstOrDefaultAsync();


		if (input is null) return NotFound();

		WasPublished = input.PublicationDate != null;
		Input = input;

		return Page();
	}

	public sealed class PostData
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required string Body { get; init; }
		public DateTimeOffset? PublicationDate { get; init; }
		public string? Tags { get; init; }
		public required ChapterMinimal? AttachedChapter { get; init; }
		public required StoryMinimal? AttachedStory { get; init; }
		public required bool Publish { get; init; }
		public required DateTimeOffset? Schedule { get; init; }
		[DisplayName("Lock")]
		public required bool IsLocked { get; init; }
	}

	public sealed class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			var now = DateTimeOffset.UtcNow;

			RuleFor(b => b.Title)
				.NotEmpty()
				.Length(CTConfig.Blogpost.MinTitleLength, CTConfig.Blogpost.MaxTitleLength);
			RuleFor(b => b.Body)
				.NotEmpty()
				.Length(CTConfig.Blogpost.MinBodyLength, CTConfig.Blogpost.MaxBodyLength);
			RuleFor(b => b.Tags)
				.HashtagsFewerThan(CTConfig.Blogpost.MaxTagsAmount)
				.HashtagsShorterThan(CTConfig.Blogpost.MaxTagLength)
				.When(b => b.Tags is not null);
			RuleFor(b => b.Schedule)
				.InclusiveBetween(now + CTConfig.Publication.MinDelay, now + CTConfig.Publication.MaxDelay)
				.When(b => b.Schedule is not null);
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (!ModelState.IsValid) return await OnGetAsync(id);

		// Get the logged-in user
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		// Get cutoff point
		var body = Input.Body.AsSpan().Trim();
		var cutoff = body.IndexOf(CTConfig.Blogpost.CutoffMarker, StringComparison.OrdinalIgnoreCase);

		if (cutoff <= 0)
		{
			cutoff = body.IndexOfBefore(' ', config.BlogpostExcerptDefaultCutoff);
		}
		else if (!User.IsStaff())
		{
			cutoff = Math.Min(body.IndexOfBefore(' ', config.BlogpostExcerptDefaultCutoff * 2), cutoff);
		}

		var schedule = Input.Schedule is {} sch
			? TimeZoneInfo.ConvertTime(sch, User.GetTimeZoneInfo()).ToUniversalTime()
			: (DateTimeOffset?)null;

		var rows = await context.Blogposts
			.Where(b => b.Id == id)
			.Where(b => b.AuthorId == uid)
			.ExecuteUpdateAsync(spc => spc
				.SetProperty(p => p.Title, Input.Title.Trim())
				.SetProperty(p => p.Slug, Input.Title.Trim().Friendlify())
				.SetProperty(p => p.Body, Input.Body.Trim())
				.SetProperty(p => p.ExcerptCutoff, cutoff > 0 ? cutoff : config.BlogpostExcerptDefaultCutoff)
				.SetProperty(p => p.WordCount, Input.Body.Words())
				.SetProperty(b => b.Hashtags, Input.Tags.ParseHashtags().ToArray())
				.SetProperty(b => b.IsVisible, Input.Publish)
				.SetProperty(b => b.IsLocked, Input.IsLocked)
				.If(Input.Publish, usb => usb
					.SetProperty(b => b.PublicationDate, b => b.PublicationDate ?? DateTimeOffset.UtcNow))
				.If(schedule is not null, usb => usb
					.SetProperty(b => b.ScheduledFor, schedule))
			);

		if (rows <= 0) return NotFound();

		return Blog_Post.Get(id, Input.Title.Trim().Friendlify()).Redirect(this);
	}
}