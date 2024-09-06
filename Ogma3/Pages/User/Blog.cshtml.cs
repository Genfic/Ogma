using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User;

public class BlogModel(UserRepository userRepo, ApplicationDbContext context)
	: PageModel
{
	private const int PerPage = 25;

	public required ICollection<BlogpostCard> Posts { get; set; }
	public required ProfileBar ProfileBar { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
	{
		var uid = User.GetNumericId();

		var profileBar = await userRepo.GetProfileBar(name);
		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		// Start building the query
		var query = context.Blogposts
			.Where(b => b.AuthorId == ProfileBar.Id);

		if (uid != ProfileBar.Id)
		{
			// If the profile page doesn't belong to the current user, apply additional filters
			query = query
				.Where(b => b.PublicationDate != null)
				.Where(b => b.ContentBlockId == null);
		}

		// Resolve query
		Posts = await query
			.OrderByDescending(b => b.CreationDate)
			.Paginate(page, PerPage)
			.ProjectToCard()
			.AsNoTracking()
			.ToListAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = PerPage,
			ItemCount = await query.CountAsync(),
			CurrentPage = page,
		};

		return Page();
	}
}