using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Ratings;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class Blacklists(ApplicationDbContext context) : PageModel
{
	public required List<Rating> Ratings { get; set; }
	public required List<UserCard> BlockedUsers { get; set; }
	public string Preselected => JsonSerializer.Serialize(BlacklistedTags, PreselectedJsonContext.Default.ListInt64);

	public async Task<IActionResult> OnGetAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		BlacklistedRatings = await context.BlacklistedRatings
			.Where(br => br.UserId == uid)
			.Select(br => br.RatingId)
			.ToListAsync();
		BlacklistedTags = await context.BlacklistedTags
			.Where(bt => bt.UserId == uid)
			.Select(bt => bt.TagId)
			.ToListAsync();
		BlockedUsers = await context.BlacklistedUsers
			.Where(bu => bu.BlockingUserId == uid)
			.Select(bu => bu.BlockedUser)
			.ProjectToCard()
			.ToListAsync();

		Ratings = await context.Ratings.ToListAsync();

		return Page();
	}

	[BindProperty] public required List<long> BlacklistedRatings { get; set; }
	[BindProperty] public required List<long> BlacklistedTags { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var user = await context.Users
			.Where(u => u.Id == uid)
			.Include(u => u.BlacklistedRatings)
			.Include(u => u.BlacklistedTags)
			.FirstOrDefaultAsync();
		if (user is null) return Unauthorized();

		// Clear blacklists
		context.BlacklistedRatings.RemoveRange(user.BlacklistedRatings);
		context.BlacklistedTags.RemoveRange(user.BlacklistedTags);

		await context.SaveChangesAsync();

		// Add blacklisted ratings
		user.BlacklistedRatings = BlacklistedRatings
			.Select(rating => new BlacklistedRating
			{
				RatingId = rating,
				UserId = uid,
			})
			.ToList();
		// And tags
		user.BlacklistedTags = BlacklistedTags
			.Select(tag => new BlacklistedTag
			{
				TagId = tag,
				UserId = uid,
			})
			.ToList();

		await context.SaveChangesAsync();

		return RedirectToPage();
	}
}

[JsonSerializable(typeof(List<long>))]
public partial class PreselectedJsonContext : JsonSerializerContext;