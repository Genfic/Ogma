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

public class StoryModel : PageModel
{
	private readonly UserRepository _userRepo;
	private readonly ApplicationDbContext _context;

	public StoryModel(UserRepository userRepo, ApplicationDbContext context)
	{
		_userRepo = userRepo;
		_context = context;
	}

	public class StoryDetails
	{
		public long Id { get; init; }
		public long AuthorId { get; init; }
		public string Title { get; init; } = null!;
		public string Slug { get; init; } = null!;
		public string Description { get; init; } = null!;
		public string Hook { get; init; } = null!;
		public string? Cover { get; init; }
		public DateTime ReleaseDate { get; init; }
		// public DateTime CreationDate { get; set; }
		public bool IsPublished { get; init; }
		public ICollection<ChapterBasicDto> Chapters { get; init; } = null!;
		public ICollection<TagDto> Tags { get; init; } = null!;
		public Rating Rating { get; init; } = null!;
		public EStoryStatus Status { get; init; }
		public int WordCount { get; init; }
		// public int FullWordCount { get; init; }
		public int ChaptersCount { get; init; }
		// public int FullChaptersCount { get; init; }
		public int CommentsCount { get; init; }
		public int Score { get; init; }
		public ContentBlockCard? ContentBlock { get; init; }
	}

	public class ChapterBasicDto
	{
		public long Id { get; init; }
		public string Slug { get; init; } = null!;
		public string Title { get; init; } = null!;
		public DateTime PublishDate { get; init; }
		public bool IsPublished { get; init; }
		public bool IsBlocked { get; init; }
		public int WordCount { get; init; }
	}

	public StoryDetails Story { get; private set; } = null!;
	public ProfileBar ProfileBar { get; private set; } = null!;

	public async Task<IActionResult> OnGetAsync(long id, string? slug)
	{
		var uid = User.GetNumericId();

		var story = await _context.Stories
			.TagWith($"Fetching story {id} — {slug}")
			.Where(s => s.Id == id)
			.Where(s => s.PublicationDate != null || s.AuthorId == uid)
			.Where(b => b.ContentBlockId == null || b.AuthorId == uid || User.IsStaff())
			.Select(MapStoryDetails(uid))
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();

		Story = story;

		ProfileBar = await _userRepo.GetProfileBar(Story.AuthorId);

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
		// FullWordCount = s.AuthorId == uid ? s.Chapters.Sum(c => c.WordCount) : 0,
		ChaptersCount = s.Chapters.Count,
		// FullChaptersCount = s.AuthorId == uid ? s.Chapters.Count : 0,
		CommentsCount = s.Chapters.Sum(c => c.CommentsThread.CommentsCount),
		IsPublished = s.PublicationDate != null,
		ReleaseDate = s.PublicationDate ?? s.CreationDate,
		// CreationDate = s.CreationDate,
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