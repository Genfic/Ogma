using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages;

public class StoryModel(UserRepository userRepo, ApplicationDbContext context) : PageModel
{
	public required StoryDetails Story { get; set; }
	public required ChapterBasic[] Chapters { get; set; }
	public required ProfileBar ProfileBar { get; set; }

	public async Task<IActionResult> OnGetAsync(long id, string? slug)
	{
		var uid = User.GetNumericId();

		var story = await context.Stories
			.TagWith($"Fetching story {id} — {slug}")
			.Where(s => s.Id == id)
			.WhereIf(s => s.PublicationDate != null || s.AuthorId == uid, uid is not null)
			.WhereIf(b => b.ContentBlockId == null || b.AuthorId == uid || User.IsStaff(), uid is not null)
			.Select(_mapStoryDetails)
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();
		Story = story;

		var profileBar = await userRepo.GetProfileBar(Story.AuthorId);

		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		Chapters = await context.Chapters
			.TagWith($"Fetching chapters for story {id} — {slug}")
			.Where(c => c.StoryId == id)
			.WhereIf(c => c.PublicationDate != null || c.Story.AuthorId == uid, uid is not null)
			.WhereIf(c => c.ContentBlockId == null || c.Story.AuthorId == uid, uid is not null)
			.OrderBy(c => c.Order)
			.Select(c => new ChapterBasic
			{
				Id = c.Id,
				Title = c.Title,
				Slug = c.Slug,
				PublishDate = c.PublicationDate ?? c.CreationDate,
				IsPublished = c.PublicationDate != null,
				IsBlocked = c.ContentBlockId != null,
				WordCount = c.WordCount,
			})
			.ToArrayAsync();
		
		return Page();
	}

	private static Expression<Func<Story, StoryDetails>> _mapStoryDetails = s => new StoryDetails
	{
		Id = s.Id,
		Title = s.Title,
		Slug = s.Slug,
		Hook = s.Hook,
		Description = s.Description,
		Cover = s.Cover,
		Rating = s.Rating,
		VotesCount = s.Votes.Count,
		Status = s.Status,
		AuthorId = s.AuthorId,
		WordCount = s.WordCount,
		ChaptersCount = s.Chapters.Count,
		CommentsCount = s.Chapters.Sum(c => c.CommentsThread.CommentsCount),
		IsPublished = s.PublicationDate != null,
		ReleaseDate = s.PublicationDate ?? s.CreationDate,
		ContentBlock = s.ContentBlock == null
			? null
			: new ContentBlockCard(s.ContentBlock.Reason, s.ContentBlock.DateTime, s.ContentBlock.Issuer.UserName),
		Tags = s.Tags
			.OrderByDescending(t => t.Namespace.HasValue)
			.ThenByDescending(t => t.Namespace)
			.Select(t => new TagDto
			{
				Id = t.Id,
				Name = t.Name,
				Slug = t.Slug,
				Namespace = t.Namespace,
				Description = t.Description,
			})
			.ToList(),
	};
}