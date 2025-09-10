using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Routes.Pages;

namespace Ogma3.Pages.Club.Forums;

[Authorize]
public sealed class Pin(ApplicationDbContext context) : PageModel
{
	public required GetData Data { get; set; }

	public sealed class GetData
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required long ClubId { get; init; }
		public required bool IsPinned { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long id)
	{
		var data = await context.ClubThreads
			.Where(ct => ct.Id == id)
			.Select(ct => new GetData
			{
				Id = ct.Id,
				ClubId = ct.ClubId,
				Title = ct.Title,
				IsPinned = ct.IsPinned,
			})
			.FirstOrDefaultAsync();

		if (data is null) return NotFound();
		Data = data;

		return Page();
	}


	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var thread = await context.ClubThreads
			.Where(ct => ct.Id == id)
			.Where(ct => ct.Club.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => new List<EClubMemberRoles>
				{
					EClubMemberRoles.Founder,
					EClubMemberRoles.Admin,
					EClubMemberRoles.Moderator,
				}.Contains(cm.Role)))
			.FirstOrDefaultAsync();

		if (thread is null) return NotFound();

		thread.IsPinned = !thread.IsPinned;
		await context.SaveChangesAsync();

		return Club_Forums_Details.Get(thread.Id, thread.ClubId).Redirect(this);
	}
}