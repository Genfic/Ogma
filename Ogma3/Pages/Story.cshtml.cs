﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages;

public sealed class StoryModel(UserRepository userRepo, ApplicationDbContext context) : PageModel
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
			.Where(s => s.PublicationDate != null || s.AuthorId == uid)
			.Where(b => b.ContentBlockId == null || b.AuthorId == uid || User.IsStaff())
			.AsSplitQuery()
			.Select(StoryMapper.MapToDetails)
			.FirstOrDefaultAsync();

		if (story is null) return NotFound();
		Story = story;

		var profileBar = await userRepo.GetProfileBar(Story.AuthorId);

		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		Chapters = await context.Chapters
			.TagWith($"Fetching chapters for story {id} — {slug}")
			.Where(c => c.StoryId == id)
			.Where(c => c.PublicationDate != null || c.Story.AuthorId == uid)
			.Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid)
			.OrderBy(c => c.Order)
			.ProjectToBasic()
			.ToArrayAsync();

		return Page();
	}
}