using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User.Library;

public sealed class LibraryModel(ApplicationDbContext context, UserRepository userRepo) : PageModel
{
	public required bool IsCurrentUser { get; set; }
	public required ProfileBar ProfileBar { get; set; }
	public required List<BookshelfCard> Shelves { get; set; }

	public async Task<IActionResult> OnGetAsync(string name)
	{
		var profileBar = await userRepo.GetProfileBar(name.ToUpper());
		if (profileBar is null) return NotFound();
		ProfileBar = profileBar;

		IsCurrentUser = string.Equals(name, User.GetUsername(), StringComparison.OrdinalIgnoreCase);

		Shelves = await context.Shelves
			.Where(s => s.OwnerId == ProfileBar.Id)
			.WhereIf(s => s.IsPublic, !IsCurrentUser)
			.Select(s => new BookshelfCard
			{
				Id = s.Id,
				Name = s.Name,
				Description = s.Description,
				IconName = s.Icon == null ? null : s.Icon.Name,
				Color = s.Color,
				StoriesCount = s.Stories.Count(st => st.PublicationDate != null),
			})
			.ToListAsync();

		return Page();
	}
}