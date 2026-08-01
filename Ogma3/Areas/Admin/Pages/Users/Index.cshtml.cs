using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Pages.Shared;

namespace Ogma3.Areas.Admin.Pages.Users;

[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public sealed class Index (ApplicationDbContext context) : PageModel
{
	private const int PerPage = 50;

	public List<UserDto> Users { get; set; } = [];
	public required Pagination Pagination { get; set; }

	public async Task<ActionResult> OnGet([FromQuery] int page = 1)
	{
		Users = await context.Users
			.Where(u => u.Id > 0)
			.Paginate(page, PerPage)
			.OrderByDescending(u => u.RegistrationDate)
			.Select(u => new UserDto(
					u.Id,
					u.UserName,
					u.RegistrationDate,
					u.Stories.Count(s => s.IsVisible),
					u.Stories.Where(s => s.IsVisible).Sum(s => s.ChapterCount),
					u.Blogposts.Count(b => b.IsVisible),
					u.EmailConfirmed && u.DeletedAt == null && u.Infractions.All(i => i.Type != InfractionType.Ban)))
			.ToListAsync();

		Pagination = new()
		{
			CurrentPage = page,
			PerPage = PerPage,
			ItemCount = await context.Users.CountAsync(u => u.Id > 0),
		};

		return Page();
	}

	public sealed record UserDto(long Id, string Name, DateTimeOffset Joined, int Stories, int Chapters, int Blogposts, bool Active);
}