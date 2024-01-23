using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Stories;

public class IndexModel(ApplicationDbContext context, OgmaConfig config, IMapper mapper) : PageModel
{
	public required List<Rating> Ratings { get; set; }
	public required List<StoryCard> Stories { get; set; }
	public required IEnumerable<long> Tags { get; set; }
	public required EStorySortingOptions SortBy { get; set; }
	public required string? SearchBy { get; set; }
	public required long? Rating { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task OnGetAsync(
		[FromQuery] IList<long> tags,
		[FromQuery] string? q = null,
		[FromQuery] EStorySortingOptions sort = EStorySortingOptions.DateDescending,
		[FromQuery] long? rating = null,
		[FromQuery] int page = 1
	)
	{
		var uid = User.GetNumericId();

		SearchBy = q;
		SortBy = sort;
		Rating = rating;
		Tags = tags;

		// Load ratings
		Ratings = await context.Ratings.ToListAsync();

		// Load stories
		var query = context.Stories
			.AsQueryable()
			.Search(tags, q, rating)
			.Where(s => s.PublicationDate != null)
			.Where(s => s.ContentBlockId == null)
			.Blacklist(context, uid);

		Stories = await query
			.SortByEnum(sort)
			.Paginate(page, config.StoriesPerPage)
			.ProjectTo<StoryCard>(mapper.ConfigurationProvider)
			.AsNoTracking()
			.ToListAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = config.StoriesPerPage,
			ItemCount = await query.CountAsync(),
			CurrentPage = page
		};
	}
}