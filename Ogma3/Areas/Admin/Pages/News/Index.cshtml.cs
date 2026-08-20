using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Pages.Shared.Cards;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Areas.Admin.Pages.News;

public sealed class IndexModel(AppDbContext ctx) : PageModel
{
	public List<NewsDto> News { get; set; } = [];

	public async Task<IActionResult> OnGetAsync()
	{
		News = await ctx.News
			.OrderByDescending(n => n.CreationDate)
			.ProjectToDto()
			.ToListAsync();

		return Page();
	}

}

public sealed record NewsDto(
	long Id,
	string Title,
	string Slug,
	DateTimeOffset CreationDate,
	DateTimeOffset? PublicationDate,
	bool IsVisible,
	string AuthorUserName
);

[Mapper]
public static partial class NewsMapper
{
	public static partial IQueryable<NewsDto> ProjectToDto(this IQueryable<Data.NewsPosts.News> query);
}