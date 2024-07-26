using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public class TagModel(ApplicationDbContext context) : PageModel
{
	private const int PerPage = 25;

	public record TagInfo(string Name, ETagNamespace? Namespace);

	public TagInfo Tag { get; private set; } = null!;
	public IList<StoryCard> Stories { get; private set; } = null!;
	public Pagination Pagination { get; private set; } = null!;

	public async Task<IActionResult> OnGetAsync(long id, string? slug, [FromQuery] int page = 1)
	{
		var uid = User.GetNumericId();

		var tag = await context.Tags
			.Where(t => t.Id == id)
			.Select(t => new TagInfo(t.Name, t.Namespace))
			.FirstOrDefaultAsync();

		if (tag is null) return NotFound();
		Tag = tag;

		var query = context.Stories
			.Where(s => s.PublicationDate != null)
			.Where(s => s.ContentBlockId == null)
			.Where(s => s.Tags.Any(st => st.Id == id))
			.Blacklist(context, uid);

		Stories = await query
			.OrderByDescending(s => s.PublicationDate)
			.Paginate(page, PerPage)
			.ProjectToCard()
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