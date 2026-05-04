using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Images;
using Ogma3.Data.Notifications;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Services.FileUploader;
using Utils.Extensions;
using Story = Routes.Pages.Story;

namespace Ogma3.Pages.Stories;

[Authorize]
public sealed class EditModel
(
	ApplicationDbContext context,
	ImageUploader uploader,
	OgmaConfig ogmaConfig,
	NotificationsRepository notificationsRepo)
	: PageModel
{
	public required List<RatingDto> Ratings { get; set; }
	public required List<TagDto> Genres { get; set; }
	public required List<TagDto> ContentWarnings { get; set; }
	public required List<TagDto> Franchises { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		// Get the story to edit and make sure the author matches logged-in user
		var input = await context.Stories
			.Where(s => s.Id == id)
			.Where(s => s.AuthorId == uid)
			.Select(story => new InputModel
			{
				Id = story.Id,
				Title = story.Title,
				Description = story.Description,
				Hook = story.Hook,
				Rating = story.Rating.Id,
				Tags = story.Tags.Select(st => st.Id).ToList(),
				Status = story.Status,
				Published = story.PublicationDate != null,
				IsLocked = story.IsLocked,
				Credits = story.Credits.Select(c => new NullableCredit(c.Role, c.Name, c.Link)).ToList(),
			})
			.FirstOrDefaultAsync();

		if (input is null) return NotFound();
		Input = input;

		// Fill Ratings dropdown
		await Hydrate();
		return Page();
	}

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required string Description { get; init; }
		public required string Hook { get; init; }
		[DataType(DataType.Upload)] public IFormFile? Cover { get; init; }
		public required long Rating { get; init; }
		public required EStoryStatus Status { get; init; }
		public required List<long> Tags { get; init; }
		public required bool Published { get; init; }
		public required bool IsLocked { get; init; }
		public List<NullableCredit> Credits { get; init; } = [];
	}

	public sealed record NullableCredit(string? Role, string? Name, string? Link);


	public sealed class InputModelValidation : AbstractValidator<InputModel>
	{
		public InputModelValidation()
		{
			RuleFor(i => i.Title)
				.NotEmpty()
				.Length(CTConfig.Story.MinTitleLength, CTConfig.Story.MaxTitleLength);
			RuleFor(i => i.Description)
				.NotEmpty()
				.Length(CTConfig.Story.MinDescriptionLength, CTConfig.Story.MaxDescriptionLength);
			RuleFor(i => i.Hook)
				.NotEmpty()
				.Length(CTConfig.Story.MinHookLength, CTConfig.Story.MaxHookLength);
			RuleFor(i => i.Cover)
				.FileSmallerThan(CTConfig.Story.CoverMaxWeight)
				.FileHasExtension(".jpg", ".jpeg", ".png");
			RuleFor(i => i.Rating).NotEmpty();
			RuleFor(i => i.Tags).NotEmpty();
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		if (!ModelState.IsValid)
		{
			await Hydrate();
			return Page();
		}

		var ratingExists = await context.Ratings
			.AnyAsync(r => r.Id == Input.Rating);

		if (!ratingExists)
		{
			ModelState.AddModelError(nameof(Input.Rating), "Unknown rating");
			await Hydrate();
			return Page();
		}

		var storyData = await context.Stories
			.Where(s => s.Id == id && s.AuthorId == uid)
			.Select(s => new
			{
				s.Id,
				s.Slug,
				s.Title,
				s.PublicationDate,
				s.ChapterCount,
				s.CoverId,
				CoverUrl = s.Cover != null ? s.Cover.Url : null,
				CoverETag = s.Cover != null ? s.Cover.ETag : null
			})
			.FirstOrDefaultAsync();

		if (storyData is null) return NotFound();

		if (storyData.PublicationDate is null && Input.Published && storyData.ChapterCount <= 0)
		{
			ModelState.AddModelError("", "You cannot publish a story with no chapters");
			await Hydrate();
			return Page();
		}

		var credits = Input.Credits
			.Where(c => c.Role is not null && c.Name is not null)
			.Select(c => new Credit
			{
				Role = c.Role!,
				Name = c.Name!,
				Link = c.Link,
			})
			.Take(25)
			.ToList();

		var newSlug = Input.Title.Friendlify();
		var publishDate = storyData.PublicationDate ?? (Input.Published ? DateTimeOffset.UtcNow : null);

		await context.Stories
			.Where(s => s.Id == id && s.AuthorId == uid)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(s => s.Title, Input.Title)
				.SetProperty(s => s.Slug, newSlug)
				.SetProperty(s => s.Description, Input.Description)
				.SetProperty(s => s.Hook, Input.Hook)
				.SetProperty(s => s.RatingId, Input.Rating)
				.SetProperty(s => s.Status, Input.Status)
				.SetProperty(s => s.IsLocked, Input.IsLocked)
				.SetProperty(s => s.PublicationDate, publishDate)
				.SetProperty(s => s.Credits, credits)
			);

		var existingTags = context.StoryTags
			.Where(st => st.StoryId == id);

		context.StoryTags.RemoveRange(existingTags);

		var newTags = Input.Tags
			.Distinct()
			.Select(tid => new StoryTag
			{
				StoryId = id,
				TagId = tid,
			});

		context.StoryTags.AddRange(newTags);

		if (Input.Cover is { Length: > 0 })
		{
			if (storyData.CoverETag is not null)
			{
				await uploader.Delete(storyData.CoverUrl!);
			}

			var file = await uploader.Upload(
				Input.Cover,
				"covers",
				ogmaConfig.StoryCoverWidth,
				ogmaConfig.StoryCoverHeight
			);

			if (storyData.CoverId is not null)
			{
				await context.Images
					.Where(i => i.Id == storyData.CoverId)
					.ExecuteUpdateAsync(s => s
						.SetProperty(i => i.Url, file.Key)
						.SetProperty(i => i.ETag, file.ETag));
			}
			else
			{
				var image = new Image
				{
					Url = file.Key,
					ETag = file.ETag,
				};
				context.Images.Add(image);
				await context.SaveChangesAsync();

				await context.Stories
					.Where(s => s.Id == id)
					.ExecuteUpdateAsync(s => s
						.SetProperty(st => st.CoverId, image.Id));
			}
		}

		if (publishDate is not null)
		{
			var notificationRecipients = await context.Users
				.Where(u => u.Following.Any(a => a.Id == uid))
				.Select(u => u.Id)
				.ToListAsync();

			await notificationsRepo.Create(
				ENotificationEvent.FollowedAuthorNewStory,
				notificationRecipients,
				Story.Get(id, newSlug).Url(Url) ?? "",
				$"A new story was published by {User.GetUsername()}: “{Input.Title}”"
			);
		}

		await context.SaveChangesAsync();

		return Story.Get(id, newSlug).Redirect(this);
	}

	private async Task Hydrate()
	{
		Ratings = await context.Ratings
			.OrderBy(r => r.Order)
			.Select(RatingMapper.ToDto)
			.ToListAsync();

		var tags = await context.Tags
			.OrderBy(t => t.Name)
			.ProjectToDto()
			.ToListAsync();

		Genres = tags.Where(t => t.Namespace == ETagNamespace.Genre).ToList();
		ContentWarnings = tags.Where(t => t.Namespace == ETagNamespace.ContentWarning).ToList();
		Franchises = tags.Where(t => t.Namespace == ETagNamespace.Franchise).ToList();
	}
}