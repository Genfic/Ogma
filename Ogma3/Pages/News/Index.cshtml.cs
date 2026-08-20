using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.News;

public sealed class IndexModel(AppDbContext context) : PageModel
{
	private const int PerPage = 20;

	public required List<NewsCard> NewsCards { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<IActionResult> OnGetAsync([FromQuery]int page = 1)
	{
		var query = context.News
			.Where(b => b.IsVisible)
			.Where(b => b.PublicationDate != null);

		NewsCards = await query
			.Paginate(page, PerPage)
			.Select(b => new NewsCard
			{
				Id = b.Id,
				Title = b.Title,
				Slug = b.Slug,
				PublicationDate = b.PublicationDate,
				AuthorUserName = b.Author.UserName,
				AuthorAvatarUrl = b.Author.Avatar.Url,
				Body = b.Body.Substring(0, b.ExcerptCutoff),
			})
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