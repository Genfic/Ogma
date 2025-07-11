﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;

namespace Ogma3.Pages.Stories;

[Authorize]
public sealed class DeleteModel(ApplicationDbContext context, ImageUploader uploader) : PageModel
{
	public sealed class GetData
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required string Slug { get; init; }
		public required DateTimeOffset? ReleaseDate { get; init; }
		public required DateTimeOffset CreationDate { get; init; }
		public required string Hook { get; init; }
		public required bool IsPublished { get; init; }
		public required EStoryStatus Status { get; init; }
		public required int VotesCount { get; init; }
		public required int ChaptersCount { get; init; }
		public required int CommentsCount { get; init; }
	}

	[BindProperty] public required GetData Story { get; set; }

	public async Task<IActionResult> OnGetAsync(int? id)
	{
		if (id is null) return NotFound();
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		// Get the story and make sure the logged-in user matches author
		var story = await context.Stories
			.Where(s => s.Id == id)
			.Where(s => s.AuthorId == uid)
			.Select(s => new GetData
			{
				Id = s.Id,
				Title = s.Title,
				Slug = s.Slug,
				ReleaseDate = s.PublicationDate,
				CreationDate = s.CreationDate,
				Hook = s.Hook,
				IsPublished = s.PublicationDate != null,
				Status = s.Status,
				VotesCount = s.Votes.Count,
				ChaptersCount = s.Chapters.Count,
				CommentsCount = s.Chapters.Sum(c => c.CommentThread.CommentsCount),
			})
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();
		Story = story;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(int? id)
	{
		if (id is null) return NotFound();
		if (User.GetNumericId() is not { } uid) return Unauthorized();
		if (User.GetUsername() is not {} uname) return Unauthorized();

		// Get the story and make sure the logged-in user matches author
		var story = await context.Stories
			.Where(s => s.Id == id)
			.Include(story => story.Cover)
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();
		if (story.AuthorId != uid) return Unauthorized();

		// Remove story
		context.Stories.Remove(story);

		// Delete cover
		if (story.Cover is { BackblazeId: not null })
		{
			await uploader.Delete(story.Cover.Url, story.Cover.BackblazeId);
		}

		// Save
		await context.SaveChangesAsync();

		return Routes.Pages.User_Stories.Get(uname).Redirect(this);
	}
}