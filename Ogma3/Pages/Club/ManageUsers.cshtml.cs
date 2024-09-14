using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club;

public sealed class ManageUsers(ApplicationDbContext context) : PageModel
{
	public required ClubData Club { get; set; }
	public required IEnumerable<UserDto> Users { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		if (User.GetNumericId() is not { } uid) return Unauthorized();

		var club = await context.Clubs
			.Where(c => c.Id == id)
			.Select(c => new ClubData(
				c.Id,
				c.Name,
				c.Slug,
				c.ClubMembers
					.Where(cm => cm.MemberId == uid)
					.Select(cm => cm.Role)
					.FirstOrDefault()
			))
			.FirstOrDefaultAsync();
		
		if (club is null) return NotFound();
		Club = club;

		var isPrivileged = Club.Role != null && Club.Role != EClubMemberRoles.User;

		Users = await context.ClubMembers
			.Where(cm => cm.ClubId == id)
			.Where(cm => cm.Member.ClubsBannedFrom.All(c => c.Id != id) || isPrivileged)
			.Select(cm => new UserDto(
				cm.MemberId,
				cm.Member.UserName,
				cm.MemberSince,
				cm.Member.Avatar,
				cm.Role,
				cm.Member.ClubsBannedFrom.Any(c => c.Id == id)
			))
			.ToListAsync();

		return Page();
	}


	public sealed record UserDto(
		long Id,
		string Name,
		DateTime JoinDate,
		string Avatar,
		EClubMemberRoles Role,
		bool IsBanned
	);

	public sealed record ClubData(
		long Id,
		string Name,
		string Slug,
		EClubMemberRoles? Role
	);
}