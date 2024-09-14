using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Data.Folders;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Club.Folders;

public sealed class CreateModel(ApplicationDbContext context, ClubRepository clubRepo) : PageModel
{
	public required long ClubId { get; set; }
	public required string Slug { get; set; }

	public async Task<IActionResult> OnGet(long clubId)
	{
		ClubId = clubId;

		if (User.GetNumericId() is not { } uid) return Unauthorized();

		// Check if admin
		var isAuthorized = await clubRepo.CheckRoles(clubId, uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isAuthorized) return Unauthorized();

		// Get slug
		var slug = await context.Clubs
			.Where(c => c.Id == clubId)
			.Select(c => c.Slug)
			.FirstOrDefaultAsync();

		if (slug is null) return NotFound();

		Slug = slug;

		return Page();
	}

	[BindProperty] public required PostData Input { get; init; }

	public sealed class PostData
	{
		public required string Name { get; init; }
		public required string Description { get; init; }
		public required long? ParentId { get; init; }
		public required EClubMemberRoles Role { get; init; }
	}

	public sealed class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			RuleFor(b => b.Name)
				.NotEmpty()
				.Length(CTConfig.CFolder.MinNameLength, CTConfig.CFolder.MaxNameLength);
			RuleFor(b => b.Description)
				.MaximumLength(CTConfig.CFolder.MaxDescriptionLength);
		}
	}

	public async Task<IActionResult> OnPostAsync(long clubId)
	{
		if (!ModelState.IsValid) return Page();

		if (User.GetNumericId() is not { } uid) return Unauthorized();

		// Check if authorized
		var isAuthorized = await clubRepo.CheckRoles(clubId, uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isAuthorized) return Unauthorized();

		context.Folders.Add(new Folder
		{
			Name = Input.Name,
			Slug = Input.Name.Friendlify(),
			Description = Input.Description,
			ClubId = clubId,
			ParentFolderId = Input.ParentId,
			AccessLevel = Input.Role,
		});
		await context.SaveChangesAsync();

		// Get slug
		var slug = await context.Clubs
			.Where(c => c.Id == clubId)
			.Select(c => c.Slug)
			.FirstOrDefaultAsync();

		return RedirectToPage("./Index", new { id = clubId, slug });
	}
}