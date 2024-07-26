using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Pages;

public class Faq(ApplicationDbContext context) : PageModel
{
	public required List<Data.Faqs.Faq> Faqs { get;  set; }

	public async Task OnGetAsync()
	{
		Faqs = await context.Faqs
			.AsNoTracking()
			.ToListAsync();
	}
}