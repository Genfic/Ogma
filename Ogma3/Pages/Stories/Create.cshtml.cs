using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Images;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.CustomValidators.FileSizeValidator;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Services;
using Ogma3.Services.FileUploader;
using Utils.Extensions;

namespace Ogma3.Pages.Stories;

[Authorize]
public sealed class CreateModel
(
	AppDbContext context,
	IFileUploader uploader,
	ImageProcessor processor,
	OgmaConfig ogmaConfig)
	: PageModel
{
	public required List<RatingDto> Ratings { get; set; }
	public required List<TagGroup> Tags { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		Input = new InputModel();

		Ratings = await context.Ratings
			.OrderBy(r => r.Order)
			.Select(RatingMapper.ToDto)
			.ToListAsync();

		var tagList = await context.Tags
			.Select(t => new { t.Id, t.Name, t.Slug, t.NamespaceId })
			.ToListAsync();

		var namespaces = await context.TagNamespaces
			.AsNoTracking()
			.ToDictionaryAsync(ns => ns.Id, ns => new { ns.Name, ns.Color, ns.Description });

		Tags = tagList
			.GroupBy(t => t.NamespaceId ?? long.MaxValue)
			.OrderBy(g => g.Key)
			.Select(g => {
				var ns = g.Key != 0 && namespaces.TryGetValue(g.Key, out var found) ? found : null;
				var tags = g.Select(t => new MicroTag(t.Id, t.Name, t.Slug))
					.OrderBy(t => t.Name)
					.ToList();
				return new TagGroup(ns?.Name, ns?.Color, ns?.Description, tags);
			})
			.ToList();

		return Page();
	}

	public sealed record TagGroup(string? Namespace, string? Color, string? Description, List<MicroTag> Tags);

	public sealed record MicroTag(long Id, string Name, string Slug);

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public string Title { get; init; } = "";
		public string Description { get; init; } = "";
		public string Hook { get; init; } = "";
		[DataType(DataType.Upload)] public IFormFile? Cover { get; init; }
		public long Rating { get; init; } = -1;
		public List<long> Tags { get; init; } = [];
		[Display(Name = "Extra tags")]
		public string ExtraTags { get; init; } = "";
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
			RuleFor(i => i.ExtraTags)
				.HashtagsFewerThan(CTConfig.Story.MaxExtraTagsCount)
				.HashtagsShorterThan(CTConfig.Story.MaxExtraTagLength);
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
			.Select(c => new Credit
			{
				Role = c.Role!,
				Name = c.Name!,
				Link = c.Link,
			})
			.Take(25)
			.ToList();

		// Split extra tags
		var extraTags = Input.ExtraTags
			.Split(',')
			.Select(t => t.Trim())
			.Where(t => t.Length > 0)
			.Take(CTConfig.Story.MaxExtraTagsCount)
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
			ExtraTags = extraTags,
			Cover = new Image
			{
				Url = "/img/placeholders/ph-250.png",
			},
			Credits = credits,
		};

		// Upload cover
		if (Input.Cover is { Length: > 0 })
		{
			var processed = await processor.ProcessAvatar(Input.Cover, ogmaConfig.StoryCoverWidth, ogmaConfig.StoryCoverHeight, false);
			var file = await uploader.Upload(processed, "covers");
			story.Cover = new Image
			{
				Url = file.Key,
				ETag = file.ETag,
			};
		}

		context.Stories.Add(story);
		await context.SaveChangesAsync();

		return Routes.Pages.Story.Get(story.Id, story.Slug).Redirect(this);
	}
}