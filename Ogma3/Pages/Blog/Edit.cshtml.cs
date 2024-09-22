using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Minimals;
using Utils.Extensions;

namespace Ogma3.Pages.Blog;

[Authorize]
public sealed class EditModel(ApplicationDbContext context) : PageModel
{
	[BindProperty]
	public required PostData Input { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		// Get logged in user
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
				Published = b.PublicationDate != null,
				PublicationDate = b.PublicationDate,
				AttachedChapter = b.AttachedChapter == null ? null : new ChapterMinimal
				{
					Id = b.AttachedChapter.Id,
					Title = b.AttachedChapter.Title,
					Slug = b.AttachedChapter.Slug,
					PublicationDate = b.AttachedChapter.PublicationDate,
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
					AuthorUserName = b.AttachedStory.Author.UserName,
				},
			})
			.FirstOrDefaultAsync();


		if (input is null) return NotFound();

		Input = input;
		Input.Published = Input.PublicationDate is not null;

		return Page();
	}

	public sealed class PostData
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required string Body { get; init; }
		public string? Tags { get; init; } = "";
		public required ChapterMinimal? AttachedChapter { get; init; }
		public required StoryMinimal? AttachedStory { get; init; }
		public required bool Published { get; set; }
		public required DateTimeOffset? PublicationDate { get; init; }
	}

	public sealed class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			RuleFor(b => b.Title)
				.NotEmpty()
				.Length(CTConfig.CBlogpost.MinTitleLength, CTConfig.CBlogpost.MaxTitleLength);
			RuleFor(b => b.Body)
				.NotEmpty()
				.Length(CTConfig.CBlogpost.MinBodyLength, CTConfig.CBlogpost.MaxBodyLength);
			RuleFor(b => b.Tags)
				.HashtagsFewerThan(CTConfig.CBlogpost.MaxTagsAmount)
				.HashtagsShorterThan(CTConfig.CBlogpost.MaxTagLength)
				.When(b => b.Tags is not null);
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (!ModelState.IsValid) return await OnGetAsync(id);

		// Get logged in user
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		var rows = await context.Blogposts
			.Where(b => b.Id == id)
			.Where(b => b.AuthorId == uid)
			.ExecuteUpdateAsync(spc => spc
				.SetProperty(p => p.Title, Input.Title.Trim())
				.SetProperty(p => p.Slug, Input.Title.Trim().Friendlify())
				.SetProperty(p => p.Body, Input.Body.Trim())
				.SetProperty(p => p.WordCount, Input.Body.Words())
				.SetProperty(b => b.Hashtags, Input.Tags.ParseHashtags())
				.SetProperty(b => b.PublicationDate, Input.Published ? DateTimeOffset.UtcNow : null)
			);

		if (rows <= 0) return NotFound();

		return RedirectToPage("./Post", new { id, slug = Input.Title.Trim().Friendlify() });
	}
}