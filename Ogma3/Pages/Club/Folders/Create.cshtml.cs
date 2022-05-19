using System.Linq;
using System.Threading.Tasks;
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

public class CreateModel : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly ClubRepository _clubRepo;

	public CreateModel(ApplicationDbContext context, ClubRepository clubRepo)
	{
		_context = context;
		_clubRepo = clubRepo;
	}

	public long ClubId { get; private set; }
	public string Slug { get; private set; }

	public async Task<IActionResult> OnGet(long clubId)
	{
		ClubId = clubId;

		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		// Check if founder
		var isFounder = await _clubRepo.CheckRoles(clubId, (long)uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isFounder) return Unauthorized();
		
		// Get slug
		Slug = await _context.Clubs
			.Where(c => c.Id == clubId)
			.Select(c => c.Slug)
			.FirstOrDefaultAsync();

		return Page();
	}

	[BindProperty] public PostData Input { get; init; }

	public class PostData
	{
		public string Name { get; init; }
		public string Description { get; init; }
		public long? ParentId { get; init; }
		public EClubMemberRoles Role { get; init; }
	}

	public class PostDataValidation : AbstractValidator<PostData>
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

		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		// Check if authorized
		var isAuthorized = await _clubRepo.CheckRoles(clubId, (long)uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);
		if (!isAuthorized) return Unauthorized();

		_context.Folders.Add(new Folder
		{
			Name = Input.Name,
			Slug = Input.Name.Friendlify(),
			Description = Input.Description,
			ClubId = clubId,
			ParentFolderId = Input.ParentId,
			AccessLevel = Input.Role
		});
		await _context.SaveChangesAsync();
		
		// Get slug
		var slug = await _context.Clubs
			.Where(c => c.Id == clubId)
			.Select(c => c.Slug)
			.FirstOrDefaultAsync();

		return RedirectToPage("./Index", new { id = clubId, slug });
	}
}