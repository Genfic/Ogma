using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Areas.Admin.Pages;

[Authorize(AuthorizationPolicies.RequireAdminRole)]
public sealed class Scripts(ApplicationDbContext ctx) : PageModel
{
	public void OnGet()
	{

	}

	public string? Message { get; set; }

	public async Task<IActionResult> OnGetUpdateStories()
	{
		var rows = await ctx.Stories.ExecuteUpdateAsync(setters => setters
			.SetProperty(s => s.VoteCount, s => s.Votes.Count)
			.SetProperty(s => s.LastUpdatedAt, s => s.Chapters.OrderByDescending(c => c.PublicationDate).First().PublicationDate));

		Message = $"{rows} stories updated";

		return Page();
	}
}