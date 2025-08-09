using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Shelves;

public sealed class Bookshelf(ApplicationDbContext context) : PageModel
{
	public BookshelfDetails Shelf { get; private set; } = null!;

	public sealed record BookshelfDetails(string Name, string Description, string? Color, string? IconName, IEnumerable<StoryCard> Stories);

	public async Task<IActionResult> OnGetAsync(long id, string? slug)
	{
		var uid = User.GetNumericId();

		var shelf = await context.Shelves
			.Where(s => s.Id == id)
			.Where(s => s.IsPublic || s.OwnerId == uid)
			.Select(s => new BookshelfDetails(
				s.Name,
				s.Description,
				s.Color,
				s.Icon == null ? null : s.Icon.Name,
				s.Stories.AsQueryable().Select(StoryMapper.MapToCard).ToList()))
			.FirstOrDefaultAsync();

		if (shelf is null) return NotFound();

		Shelf = shelf;

		return Page();
	}
}