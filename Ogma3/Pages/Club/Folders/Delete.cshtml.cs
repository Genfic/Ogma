using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Folders;

public sealed class DeleteModel(ApplicationDbContext context, ClubRepository clubRepo) : PageModel
{
	[BindProperty] public required DeleteViewModel Folder { get; set; }

	public sealed class DeleteViewModel
	{
		public required string Name { get; init; }
		public required string Slug { get; init; }
		public required string? Description { get; init; }
		public required int StoriesCount { get; init; }
		public required long ClubId { get; init; }
		public required long Id { get; init; }
	}

	public async Task<IActionResult> OnGet(long clubId, long id)
	{
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		// Check if authorized
		var isAuthorized = await clubRepo.CheckRoles(clubId, uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isAuthorized) return Unauthorized();

		var folder = await context.Folders
			.Where(f => f.ClubId == clubId)
			.Where(f => f.Id == id)
			.Select(f => new DeleteViewModel
			{
				Id = f.Id,
				ClubId = f.ClubId,
				Name = f.Name,
				Slug = f.Slug,
				Description = f.Description,
				StoriesCount = f.Stories.Count,
			})
			.FirstOrDefaultAsync();

		if (folder is null) return NotFound();

		Folder = folder;

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long clubId, long? id)
	{
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		// Check if authorized
		var isAuthorized = await clubRepo.CheckRoles(clubId, uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isAuthorized) return Unauthorized();

		var folder = await context.Folders
			.Where(f => f.ClubId == clubId)
			.Where(f => f.Id == id)
			.FirstOrDefaultAsync();

		if (folder is null) return NotFound();


		context.Folders.Remove(folder);
		await context.SaveChangesAsync();

		// Get slug
		var slug = await context.Clubs
			.Where(c => c.Id == clubId)
			.Select(c => c.Slug)
			.FirstOrDefaultAsync();

		return RedirectToPage("./Index", new { id = clubId, slug });
	}
}