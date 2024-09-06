using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Pages.Shelves;

public class Bookshelf(ApplicationDbContext context) : PageModel
{
	public BookshelfDetails Shelf { get; private set; } = null!;

	public sealed record BookshelfDetails(string Name, string Description, string Color, string IconName, ICollection<StoryCard> Stories);

	public async Task<IActionResult> OnGetAsync(int id, string? slug)
	{
		var uid = User.GetNumericId();

		var shelf = await context.Shelves
			.Where(s => s.Id == id)
			.Where(s => s.IsPublic || s.OwnerId == uid)
			.ProjectToDetails()
			.FirstOrDefaultAsync();

		if (shelf is null) return NotFound();

		Shelf = shelf;

		return Page();
	}
}

[Mapper]
public static partial class BoohshelfMapper
{
	public static partial IQueryable<Bookshelf.BookshelfDetails> ProjectToDetails(this IQueryable<Shelf> queryable);
}