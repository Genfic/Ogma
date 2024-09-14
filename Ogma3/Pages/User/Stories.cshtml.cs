using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User;

public sealed class StoriesModel(UserRepository userRepo, ApplicationDbContext context)
	: PageModel
{
	private const int PerPage = 25;

	public required IList<StoryCard> Stories { get; set; }
	public required ProfileBar ProfileBar { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
	{
		var uid = User.GetNumericId();

		var profileBar = await userRepo.GetProfileBar(name.ToUpper());
		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		// Start building the query
		var query = context.Stories
			.Where(b => b.AuthorId == ProfileBar.Id);

		if (name != User.GetUsername())
		{
			// If the profile page doesn't belong to the current user, apply additional filters
			query = query
				.Where(s => s.PublicationDate != null)
				.Where(s => s.ContentBlockId == null)
				.Blacklist(context, uid);
		}

		// Resolve query
		Stories = await query
			.SortByEnum(EStorySortingOptions.DateDescending)
			.Paginate(page, PerPage)
			.ProjectToCard()
			.AsNoTracking()
			.ToListAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			CurrentPage = page,
			ItemCount = await query.CountAsync(),
			PerPage = PerPage,
		};

		return Page();
	}
}