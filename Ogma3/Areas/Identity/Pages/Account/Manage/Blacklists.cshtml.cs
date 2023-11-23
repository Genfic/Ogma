using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Ratings;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class Blacklists : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public Blacklists(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public required IEnumerable<Rating> Ratings { get; set; }
	public required IEnumerable<UserCard> BlockedUsers { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		var uid = User.GetNumericId();
		if (uid == null) return Unauthorized();

		BlacklistedRatings = await _context.BlacklistedRatings
			.Where(br => br.UserId == uid)
			.Select(br => br.RatingId)
			.ToListAsync();
		BlacklistedTags = await _context.BlacklistedTags
			.Where(bt => bt.UserId == uid)
			.Select(bt => bt.TagId)
			.ToListAsync();
		BlockedUsers = await _context.BlacklistedUsers
			.Where(bu => bu.BlockingUserId == uid)
			.Select(bu => bu.BlockedUser)
			.ProjectTo<UserCard>(_mapper.ConfigurationProvider)
			.ToListAsync();

		Ratings = await _context.Ratings.ToListAsync();

		return Page();
	}

	[BindProperty] public required IEnumerable<long> BlacklistedRatings { get; set; }
	[BindProperty] public required IEnumerable<long> BlacklistedTags { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		var uid = User.GetNumericId();
		if (uid == null) return Unauthorized();

		var user = await _context.Users
			.Where(u => u.Id == uid)
			.Include(u => u.BlacklistedRatings)
			.Include(u => u.BlacklistedTags)
			.FirstOrDefaultAsync();
		if (user == null) return Unauthorized();

		// Clear blacklists
		_context.BlacklistedRatings.RemoveRange(user.BlacklistedRatings);
		_context.BlacklistedTags.RemoveRange(user.BlacklistedTags);

		await _context.SaveChangesAsync();

		// Add blacklisted ratings
		user.BlacklistedRatings = BlacklistedRatings
			.Select(rating => new BlacklistedRating
			{
				RatingId = rating,
				UserId = (long)uid
			})
			.ToList();
		// And tags
		user.BlacklistedTags = BlacklistedTags
			.Select(tag => new BlacklistedTag
			{
				TagId = tag,
				UserId = (long)uid
			})
			.ToList();

		await _context.SaveChangesAsync();

		return RedirectToPage();
	}
}