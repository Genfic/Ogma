#nullable enable


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Clubs;

public class IndexModel : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;
	private readonly OgmaConfig _config;

	public IndexModel(ApplicationDbContext context, IMapper mapper, OgmaConfig config)
	{
		_context = context;
		_mapper = mapper;
		_config = config;
	}


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

		var query = _context.Clubs.AsQueryable();

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
				EClubSortingOptions.StoriesAscending => query.OrderBy(c => c.Folders.Sum(f => f.StoriesCount)),
				EClubSortingOptions.StoriesDescending => query.OrderByDescending(c => c.Folders.Sum(f => f.StoriesCount)),
				EClubSortingOptions.ThreadsAscending => query.OrderBy(c => c.Threads.Count),
				EClubSortingOptions.ThreadsDescending => query.OrderByDescending(c => c.Threads.Count),
				EClubSortingOptions.CreationDateAscending => query.OrderBy(c => c.CreationDate),
				EClubSortingOptions.CreationDateDescending => query.OrderByDescending(c => c.CreationDate),
				_ => query.OrderByDescending(c => c.CreationDate)
			})
			.Paginate(page, _config.ClubsPerPage)
			.ProjectTo<ClubCard>(_mapper.ConfigurationProvider)
			.AsNoTracking()
			.ToListAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = _config.ClubsPerPage,
			ItemCount = await query.CountAsync(),
			CurrentPage = page
		};
	}
}