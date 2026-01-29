using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Clubs;

public sealed class IndexModel(ApplicationDbContext context, OgmaConfig config) : PageModel
{
	public IList<ClubCard> Clubs { get; private set; } = null!;
	public string? Query { get; private set; }
	public EClubSortingOptions SortBy { get; set; }
	public Pagination Pagination { get; private set; } = null!;

	public async Task OnGetAsync(
		[FromQuery] int page = 1,
		[FromQuery] string? q = null,
		[FromQuery] EClubSortingOptions sort = EClubSortingOptions.CreationDateDescending
	)
	{
		Query = q;
		SortBy = sort;

		var query = context.Clubs.AsQueryable();

		if (!string.IsNullOrEmpty(q))
		{
			query = query.Where(c => EF.Functions.Like(c.Name.ToUpper(), $"%{q.Trim().ToUpper()}%"));
		}

		Clubs = await (sort switch
			{
				EClubSortingOptions.NameAscending => query.OrderBy(c => c.Name),
				EClubSortingOptions.NameDescending => query.OrderByDescending(c => c.Name),
				EClubSortingOptions.MembersAscending => query.OrderBy(c => c.ClubMembers.Count),
				EClubSortingOptions.MembersDescending => query.OrderByDescending(c => c.ClubMembers.Count),
				EClubSortingOptions.StoriesAscending => query.OrderBy(c => c.Folders.Sum(f => f.Stories.Count)),
				EClubSortingOptions.StoriesDescending => query.OrderByDescending(c => c.Folders.Sum(f => f.Stories.Count)),
				EClubSortingOptions.ThreadsAscending => query.OrderBy(c => c.Threads.Count),
				EClubSortingOptions.ThreadsDescending => query.OrderByDescending(c => c.Threads.Count),
				EClubSortingOptions.CreationDateAscending => query.OrderBy(c => c.CreationDate),
				EClubSortingOptions.CreationDateDescending => query.OrderByDescending(c => c.CreationDate),
				_ => query.OrderByDescending(c => c.CreationDate),
			})
			.Paginate(page, config.ClubsPerPage)
			.Select(c => new ClubCard
			{
				Id = c.Id,
				Name = c.Name,
				Slug = c.Slug,
				Hook = c.Hook,
				Icon = c.Icon.Url,
				StoriesCount = c.Folders.Sum(f => f.Stories.Count),
				ThreadsCount = c.Threads.Count,
				ClubMembersCount = c.ClubMembers.Count,
			})
			.ToListAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = config.ClubsPerPage,
			ItemCount = await query.CountAsync(),
			CurrentPage = page,
		};
	}
}