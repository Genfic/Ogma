using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
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
public sealed class EditModel(ApplicationDbContext context, ImageUploader uploader, OgmaConfig ogmaConfig)
	: PageModel
{
	public required List<RatingDto> Ratings { get; set; }
	public required List<TagDto> Genres { get; set; }
	public required List<TagDto> ContentWarnings { get; set; }
	public required List<TagDto> Franchises { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		// Get logged in user
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		// Get story to edit and make sure author matches logged-in user
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
		public List<NullableCredit> Credits { get; init; } = [];
	}

	public sealed record NullableCredit(string? Role, string? Name, string? Link);
	

	public sealed class InputModelValidation : AbstractValidator<InputModel>
	{
		public InputModelValidation()
		{
			RuleFor(i => i.Title)
				.NotEmpty()
				.Length(CTConfig.CStory.MinTitleLength, CTConfig.CStory.MaxTitleLength);
			RuleFor(i => i.Description)
				.NotEmpty()
				.Length(CTConfig.CStory.MinDescriptionLength, CTConfig.CStory.MaxDescriptionLength);
			RuleFor(i => i.Hook)
				.NotEmpty()
				.Length(CTConfig.CStory.MinHookLength, CTConfig.CStory.MaxHookLength);
			RuleFor(i => i.Cover)
				.FileSmallerThan(CTConfig.CStory.CoverMaxWeight)
				.FileHasExtension(".jpg", ".jpeg", ".png");
			RuleFor(i => i.Rating).NotEmpty();
			RuleFor(i => i.Tags).NotEmpty();
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{

		// Get logged in user
		if (User.GetNumericId() is not {} uid) return Unauthorized();
		
		if (!ModelState.IsValid)
		{
			await Hydrate();
			return Page();
		}

		var rating = await context.Ratings.FindAsync(Input.Rating);

		if (rating is null)
		{
			ModelState.AddModelError(nameof(Input.Rating), "Unknown rating");
			await Hydrate();
			return Page();
		}

		var tags = await context.Tags
			.Where(t => Input.Tags.Contains(t.Id))
			.ToListAsync();

		// Get the story and make sure the logged-in user matches author
		var story = await context.Stories
			.Include(s => s.Tags)
			.Include(s => s.Rating)
			.FirstOrDefaultAsync(s => s.Id == id && s.AuthorId == uid);

		// 404 if none found
		if (story is null) return NotFound();

		// Check if it can be published
		if (story.PublicationDate is null && Input.Published && story.ChapterCount <= 0)
		{
			ModelState.AddModelError("", "You cannot publish a story with no chapters");
			await Hydrate();
			return Page();
		}

		var credits = Input.Credits
			.Where(c => c.Role is not null)
			.Where(c => c.Name is not null)
			.Select(c => new Credit(c.Role!, c.Name!, c.Link))
			.ToList();

		// Update story
		story.Title = Input.Title;
		story.Slug = Input.Title.Friendlify();
		story.Description = Input.Description;
		story.Hook = Input.Hook;
		story.Rating = rating;
		story.Tags = tags;
		story.Status = Input.Status;
		story.PublicationDate = Input.Published ? DateTimeOffset.UtcNow : null;
		story.Credits = credits;

		// Handle cover upload
		if (Input.Cover is { Length: > 0 })
		{
			// Upload cover
			var file = await uploader.Upload(
				Input.Cover,
				"covers",
				ogmaConfig.StoryCoverWidth,
				ogmaConfig.StoryCoverHeight
			);
			story.CoverId = file.FileId;
			story.Cover = Path.Join(ogmaConfig.Cdn, file.Path);
		}
		
		await context.SaveChangesAsync();

		return Routes.Pages.Story.Get(story.Id, story.Slug).Redirect(this);
	}

	private async Task Hydrate()
	{
		Ratings = await context.Ratings
			.OrderBy(r => r.Order)
			.ProjectToDto()
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