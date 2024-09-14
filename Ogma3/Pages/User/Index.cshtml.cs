using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Comments;
using Ogma3.Data.Users;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.User;

public sealed class IndexModel(ApplicationDbContext context, UserRepository userRepo)
	: PageModel
{
	public required ProfileBar ProfileBar { get; set; }
	public required ProfileDetails Data { get; set; }

	public sealed class ProfileDetails
	{
		public required string? Bio { get; init; }
		public required List<string> Links { get; init; }
		public required CommentsThreadDto CommentsThread { get; init; }
	}

	public async Task<IActionResult> OnGetAsync(string name)
	{
		var data = await context.Users
			.Where(u => u.NormalizedUserName == name.Normalize().ToUpperInvariant())
			.Select(u => new ProfileDetails
			{
				Bio = u.Bio,
				Links = u.Links,
				CommentsThread = new CommentsThreadDto(u.CommentsThread.Id, CommentSource.Profile, u.CommentsThread.LockDate),
			})
			.FirstOrDefaultAsync();

		if (data is null) return NotFound();
		Data = data;

		var bar = await userRepo.GetProfileBar(name.ToUpper());
		if (bar is null) return NotFound();
		ProfileBar = bar;
		
		return Page();
	}
}