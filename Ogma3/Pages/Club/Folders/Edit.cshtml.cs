using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Club.Folders;

public sealed class EditModel(ApplicationDbContext context, ClubRepository clubRepo) : PageModel
{
	public required string Slug { get; set; }

	public async Task<IActionResult> OnGet(long clubId, long id)
	{
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		// Check if founder or admin
		var isFounder = await clubRepo.CheckRoles(clubId, uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isFounder) return Unauthorized();

		var input = await context.Folders
			.Where(f => f.ClubId == clubId)
			.Where(f => f.Id == id)
			.Select(f => new InputModel
			{
				Id = f.Id,
				ClubId = f.ClubId,
				Name = f.Name,
				Description = f.Description,
				Role = f.AccessLevel,
			})
			.FirstOrDefaultAsync();

		if (input is null) return NotFound();

		Input = input;

		// Get slug
		var slug = await context.Clubs
			.Where(c => c.Id == clubId)
			.Select(c => c.Slug)
			.FirstOrDefaultAsync();

		if (slug is null) return NotFound();

		Slug = slug;

		return Page();
	}

	[BindProperty] public required InputModel Input { get; set; }

	public sealed class InputModel
	{
		public required long Id { get; init; }
		public required long ClubId { get; init; }
		public required string Name { get; init; }
		public required string? Description { get; init; }
		public required EClubMemberRoles Role { get; init; }
	}

	public sealed class PostDataValidation : AbstractValidator<InputModel>
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

	public async Task<IActionResult> OnPostAsync(long clubId, long id)
	{
		if (!ModelState.IsValid) return Page(); // await OnGet(clubId, id);

		if (User.GetNumericId() is not { } uid) return Unauthorized();

		// Check if authorized
		var isAuthorized = await clubRepo.CheckRoles(clubId, uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isAuthorized) return Unauthorized();

		var folder = await context.Folders
			.Where(f => f.ClubId == clubId)
			.Where(f => f.Id == Input.Id)
			.FirstOrDefaultAsync();

		if (folder is null) return NotFound();

		folder.Name = Input.Name;
		folder.Slug = Input.Name.Friendlify();
		folder.AccessLevel = Input.Role;
		folder.Description = Input.Description;


		await context.SaveChangesAsync();

		return Routes.Pages.Club_Folders_Folder.Get(clubId, folder.Id, folder.Slug).Redirect(this);
	}
}