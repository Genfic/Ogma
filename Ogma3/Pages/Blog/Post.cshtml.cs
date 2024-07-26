using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Pages.Blog;

public class DetailsModel(UserRepository userRepo, ApplicationDbContext context) : PageModel
{
	public required Details Blogpost { get; set; }
	public required ProfileBar ProfileBar { get; set; }
	public required bool IsUnavailable { get; set; }

	public class Details
	{
		public required long Id { get; init; }
		public required long AuthorId { get; init; }
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public DateTime? PublicationDate { get; init; }
		public required string Body { get; init; }
		public required IEnumerable<string> Hashtags { get; init; }
		public required CommentsThreadDto CommentsThread { get; init; }
		public ChapterMinimal? AttachedChapter { get; init; }
		public StoryMinimal? AttachedStory { get; init; }
		public ContentBlockCard? ContentBlock { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long id, string? slug)
	{
		var uid = User.GetNumericId();

		var blogpost = await context.Blogposts
			.TagWith($"Get blogpost -> {id}")
			.Where(b => b.Id == id)
			.Where(b => b.PublicationDate != null || b.AuthorId == uid)
			.Where(b => b.ContentBlockId == null || b.AuthorId == uid || User.IsStaff())
			.Select(MapDetails)
			.FirstOrDefaultAsync();

		if (blogpost is null) return NotFound();

		Blogpost = blogpost;
		
		if (Blogpost.AttachedChapter is not null && Blogpost.AttachedChapter.PublicationDate is null)
		{
			IsUnavailable = true;
		}
		else if (Blogpost.AttachedStory is not null && Blogpost.AttachedStory.PublicationDate is null)
		{
			IsUnavailable = true;
		}

		var profileBar = await userRepo.GetProfileBar(Blogpost.AuthorId);

		if (profileBar is null) return NotFound();

		ProfileBar = profileBar;
		
		return Page();
	}

	private static Expression<Func<Blogpost, Details>> MapDetails => b => new Details
	{
		Id = b.Id,
		AuthorId = b.AuthorId,
		Title = b.Title,
		Slug = b.Slug,
		Body = b.Body,
		Hashtags = b.Hashtags,
		PublicationDate = b.PublicationDate,
		CommentsThread = new CommentsThreadDto(b.CommentsThread.Id, nameof(Data.Blogposts.Blogpost), b.CommentsThread.LockDate),
		ContentBlock = b.ContentBlock == null
			? null
			: new ContentBlockCard(b.ContentBlock.Reason, b.ContentBlock.DateTime, b.ContentBlock.Issuer.UserName),
		AttachedChapter = b.AttachedChapter == null
			? null
			: new ChapterMinimal
			{
				Id = b.AttachedChapter.Id,
				Title = b.AttachedChapter.Title,
				Slug = b.AttachedChapter.Slug,
				PublicationDate = b.AttachedChapter.PublicationDate,
				StoryTitle = b.AttachedChapter.Story.Title,
				StoryId = b.AttachedChapter.StoryId,
				StoryAuthorUserName = b.AttachedChapter.Story.Author.UserName
			},
		AttachedStory = b.AttachedStory == null
			? null
			: new StoryMinimal
			{
				Id = b.AttachedStory.Id,
				Title = b.AttachedStory.Title,
				Slug = b.AttachedStory.Slug,
				PublicationDate = b.AttachedStory.PublicationDate,
				AuthorUserName = b.AttachedStory.Author.UserName
			}
	};
}