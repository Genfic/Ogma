using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Areas.Admin.Pages;

public sealed class Index(ApplicationDbContext context) : PageModel
{
	public sealed record CountItem(int Count, string Name);

	public required List<CountItem> Counts { get; set; }

	public async Task OnGet()
	{
		Counts = await context.Database.SqlQueryRaw<CountItem>("""
		     	SELECT 'Stories' as name, count(1) as count FROM "Stories" UNION
		     	SELECT 'Chapters' as name, count(1) as count FROM "Chapters" UNION
		     	SELECT 'Blogposts' as name, count(1) as count FROM "Blogposts" UNION
		     	SELECT 'Users' as name, count(1) as count FROM "AspNetUsers" u WHERE u."EmailConfirmed" UNION
		     	SELECT 'Comments' as name, count(1) as count FROM "Comments" c WHERE c."DeletedBy" is null UNION
		     	SELECT 'Reports' as name, count(1) as count FROM "Reports";
		     """)
			.ToListAsync();
	}
}