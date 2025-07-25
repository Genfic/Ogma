using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Ratings;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public sealed class Blacklists(ApplicationDbContext context) : PageModel
{
	public required List<Rating> Ratings { get; set; }
	public required List<UserCard> BlockedUsers { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		BlacklistedRatings = await context.BlacklistedRatings
			.Where(br => br.UserId == uid)
			.Select(br => br.RatingId)
			.ToListAsync();
		var blacklistedTags = await context.BlacklistedTags
			.Where(bt => bt.UserId == uid)
			.Select(bt => bt.Tag.Name)
			.ToListAsync();
		BlacklistedTags = string.Join(' ', blacklistedTags.Select(t => t.Trim(':').ToLower()));
		BlockedUsers = await context.BlockedUsers
			.Where(bu => bu.BlockingUserId == uid)
			.Select(bu => bu.BlockedUser)
			.Select(UserMappings.ToUserCard)
			.ToListAsync();

		Ratings = await context.Ratings.ToListAsync();

		return Page();
	}

	[BindProperty] public required List<long> BlacklistedRatings { get; set; }
	[BindProperty] public required string? BlacklistedTags { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		if (User.GetNumericId() is not {} uid) return Unauthorized();

		var user = await context.Users
			.Where(u => u.Id == uid)
			.Include(u => u.BlacklistedRatings)
			.Include(u => u.BlacklistedTags)
			.FirstOrDefaultAsync();
		if (user is null) return Unauthorized();

		// Clear the blacklists
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
		var blacklistedTags = BlacklistedTags?.Split(' ') ?? [];
		var tagsToBlacklist = await context.Tags
			.Where(t => blacklistedTags.Contains(t.Name.ToLower()))
			.Select(t => t.Id)
			.ToListAsync();
		// And add them to the blacklist
		user.BlacklistedTags = tagsToBlacklist.Select(tag => new BlacklistedTag
			{
				TagId = tag,
				UserId = uid,
			})
			.ToList();

		await context.SaveChangesAsync();

		return RedirectToPage();
	}

	public sealed record RatingDto(long Id, RatingIcon Icon);
}