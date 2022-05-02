using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.FileUploader;
using Serilog;

namespace Ogma3.Pages.Clubs;

[Authorize]
public class DeleteModel : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly ImageUploader _uploader;

	public DeleteModel(ApplicationDbContext context, ImageUploader uploader)
	{
		_context = context;
		_uploader = uploader;
	}

	[BindProperty] public GetData Club { get; set; }

	public class GetData
	{
		public long Id { get; init; }
		public string Name { get; init; }
		public string Slug { get; init; }
		public string Hook { get; init; }
		public DateTime CreationDate { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long? id)
	{
		if (id is null) return NotFound();

		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		Club = await _context.Clubs
			.Where(c => c.Id == id)
			.Where(c => c.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
			.Select(c => new GetData
			{
				Id = c.Id,
				Name = c.Name,
				Slug = c.Slug,
				Hook = c.Hook,
				CreationDate = c.CreationDate
			})
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (Club is null) return NotFound();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long? id)
	{
		if (id is null) return NotFound();

		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		Log.Information("User {UserId} attempted to delete club {ClubId}", uid, id);

		var club = await _context.Clubs
			.Where(c => c.Id == id)
			.Where(c => c.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => cm.Role == EClubMemberRoles.Founder || cm.Role == EClubMemberRoles.Admin))
			.FirstOrDefaultAsync();

		if (club is null)
		{
			Log.Information("User {UserId} did not succeed in deleting club {ClubId}", uid, id);
			return NotFound();
		}


		Log.Information("User {UserId} succeeded in deleting club {ClubId}", uid, id);
		_context.Clubs.Remove(club);

		if (club.Icon is not null && club.IconId is not null)
		{
			await _uploader.Delete(club.Icon, club.IconId);
		}

		await _context.SaveChangesAsync();

		return RedirectToPage("./Index");
	}
}