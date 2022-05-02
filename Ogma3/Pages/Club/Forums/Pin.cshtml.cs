using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club.Forums;

[Authorize]
public class Pin : PageModel
{
	private readonly ApplicationDbContext _context;

	public Pin(ApplicationDbContext context)
	{
		_context = context;
	}

	public GetData Data { get; set; }

	public class GetData
	{
		public long Id { get; init; }
		public string Title { get; init; }
		public long ClubId { get; init; }
		public bool IsPinned { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(long id)
	{
		Data = await _context.ClubThreads
			.Where(ct => ct.Id == id)
			.Select(ct => new GetData
			{
				Id = ct.Id,
				ClubId = ct.ClubId,
				Title = ct.Title,
				IsPinned = ct.IsPinned
			})
			.FirstOrDefaultAsync();

		if (Data is null) return NotFound();

		return Page();
	}


	public async Task<IActionResult> OnPostAsync(long id)
	{
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		var thread = await _context.ClubThreads
			.Where(ct => ct.Id == id)
			.Where(ct => ct.Club.ClubMembers
				.Where(cm => cm.MemberId == uid)
				.Any(cm => new[]
				{
					EClubMemberRoles.Founder,
					EClubMemberRoles.Admin,
					EClubMemberRoles.Moderator
				}.Contains(cm.Role)))
			.FirstOrDefaultAsync();

		if (thread is null) return NotFound();

		thread.IsPinned = !thread.IsPinned;
		await _context.SaveChangesAsync();

		return RedirectToPage("./Details", new { clubId = thread.ClubId, threadId = thread.Id });
	}
}