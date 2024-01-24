using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages;

public class StoryModel(UserRepository userRepo, ApplicationDbContext context) : PageModel
{
	public class StoryDetails
	{
		public required long Id { get; init; }
		public required long AuthorId { get; init; }
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public required string Description { get; init; }
		public required string Hook { get; init; }
		public required string Cover { get; init; }
		public required DateTime ReleaseDate { get; init; }
		public required bool IsPublished { get; init; }
		public required ICollection<ChapterBasicDto> Chapters { get; init; }
		public required ICollection<TagDto> Tags { get; init; }
		public required Rating Rating { get; init; }
		public required EStoryStatus Status { get; init; }
		public required int WordCount { get; init; }
		public required int ChaptersCount { get; init; }
		public required int CommentsCount { get; init; }
		public required int Score { get; init; }
		public required ContentBlockCard? ContentBlock { get; init; }
	}

	public class ChapterBasicDto
	{
		public required long Id { get; init; }
		public required string Slug { get; init; }
		public required string Title { get; init; }
		public required DateTime PublishDate { get; init; }
		public required bool IsPublished { get; init; }
		public required bool IsBlocked { get; init; }
		public required int WordCount { get; init; }
	}

	public required StoryDetails Story { get; set; }
	public required ProfileBar ProfileBar { get; set; }

	public async Task<IActionResult> OnGetAsync(long id, string? slug)
	{
		var uid = User.GetNumericId();

		var story = await context.Stories
			.TagWith($"Fetching story {id} — {slug}")
			.Where(s => s.Id == id)
			.Where(s => s.PublicationDate != null || s.AuthorId == uid)
			.Where(b => b.ContentBlockId == null || b.AuthorId == uid || User.IsStaff())
			.Select(MapStoryDetails(uid))
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();

		Story = story;

		var profileBar = await userRepo.GetProfileBar(Story.AuthorId);

		if (profileBar is null) return NotFound();

		ProfileBar = profileBar;
		
		return Page();
	}

	private static Expression<Func<Story, StoryDetails>> MapStoryDetails(long? uid) => s => new StoryDetails
	{
		Id = s.Id,
		Title = s.Title,
		Slug = s.Slug,
		Hook = s.Hook,
		Description = s.Description,
		Cover = s.Cover,
		Rating = s.Rating,
		Score = s.Votes.Count,
		Status = s.Status,
		AuthorId = s.AuthorId,
		WordCount = s.WordCount,
		ChaptersCount = s.Chapters.Count,
		CommentsCount = s.Chapters.Sum(c => c.CommentsThread.CommentsCount),
		IsPublished = s.PublicationDate != null,
		ReleaseDate = s.PublicationDate ?? s.CreationDate,
		ContentBlock = s.ContentBlock == null
			? null
			: new ContentBlockCard
			{
				Reason = s.ContentBlock.Reason,
				DateTime = s.ContentBlock.DateTime,
				IssuerUserName = s.ContentBlock.Issuer.UserName
			},
		Tags = s.Tags
			.OrderByDescending(t => t.Namespace.HasValue)
			.ThenByDescending(t => t.Namespace)
			.Select(t => new TagDto
			{
				Id = t.Id,
				Name = t.Name,
				Slug = t.Slug,
				Namespace = t.Namespace,
				Description = t.Description
			})
			.ToList(),
		Chapters = s.Chapters
			.Where(c => c.PublicationDate != null || c.Story.AuthorId == uid)
			.Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid)
			.OrderBy(c => c.Order)
			.Select(c => new ChapterBasicDto
			{
				Id = c.Id,
				Title = c.Title,
				Slug = c.Slug,
				PublishDate = c.PublicationDate ?? c.CreationDate,
				IsPublished = c.PublicationDate != null,
				IsBlocked = c.ContentBlockId != null,
				WordCount = c.WordCount
			})
			.ToList()
	};
}