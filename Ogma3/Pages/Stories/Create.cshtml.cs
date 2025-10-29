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
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Stories;

[Authorize]
public sealed class CreateModel(
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

	public async Task<IActionResult> OnGetAsync()
	{
		Input = new InputModel();

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

		return Page();
	}

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public string Title { get; init; } = "";
		public string Description { get; init; } = "";
		public string Hook { get; init; } = "";
		[DataType(DataType.Upload)] public IFormFile? Cover { get; init; }
		public long Rating { get; init; } = -1;
		public List<long> Tags { get; init; } = [];
		public List<NullableCredit> Credits { get; init; } = [];
	}

	public sealed record NullableCredit(string? Role, string? Name, string? Link)
	{
		public static readonly NullableCredit Empty = new("", "", "");
	}

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
			RuleFor(i => i.Credits).Must(p => p.Count <= 10);
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid) return await OnGetAsync();

		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var tags = await context.Tags
			.Where(t => Input.Tags.Contains(t.Id))
			.ToListAsync();

		var credits = Input.Credits
			.Where(c => c.Role is not null)
			.Where(c => c.Name is not null)
			.Select(c => new Credit(c.Role!, c.Name!, c.Link))
			.ToList();

		// Add a story
		var story = new Story
		{
			AuthorId = uid,
			Title = Input.Title,
			Slug = Input.Title.Friendlify(),
			Description = Input.Description,
			Hook = Input.Hook,
			RatingId = Input.Rating,
			Tags = tags,
			Cover = new Image
			{
				Url = "/img/placeholders/ph-250.png",
			},
			Credits = credits,
		};

		// Upload cover
		if (Input.Cover is { Length: > 0 })
		{
			var file = await uploader.Upload(
				Input.Cover,
				"covers",
				ogmaConfig.StoryCoverWidth,
				ogmaConfig.StoryCoverHeight
			);
			story.Cover = new Image
			{
				Url = Path.Join(ogmaConfig.Cdn, file.Path),
				BackblazeId = file.FileId,
			};
		}

		context.Stories.Add(story);
		await context.SaveChangesAsync();

		// Get a list of users that should receive notifications
		var notificationRecipients = await context.Users
			.Where(u => u.Following.Any(a => a.Id == uid))
			.Select(u => u.Id)
			.ToListAsync();

		// Notify
		await notificationsRepo.Create(ENotificationEvent.FollowedAuthorNewStory,
			notificationRecipients,
			"/Story",
			new { story.Id, story.Slug });

		return Routes.Pages.Story.Get(story.Id, story.Slug).Redirect(this);
	}
}