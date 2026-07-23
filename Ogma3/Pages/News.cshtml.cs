using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public sealed class NewsModel(ApplicationDbContext context) : PageModel
{
	private const int PerPage = 20;

	public required List<NewsCard> NewsCards { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<IActionResult> OnGetAsync([FromQuery]int page = 1)
	{
		var query = context.Blogposts
			.Where(p => p.Hashtags.AsQueryable().Contains("site-news"))
			.Where(p => p.Author.Roles.Any(r => r.IsStaff));

		NewsCards = await query
			.Paginate(page, PerPage)
			.ProjectToNewsCard()
			.ToListAsync();

		Pagination = new()
		{
			CurrentPage = page,
			ItemCount = await query.CountAsync(),
			PerPage = PerPage,
		};

		return Page();
	}
}