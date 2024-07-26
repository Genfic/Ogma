using System.Text.Json;
using System.Text.Json.Serialization;
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

public class IndexModel(ApplicationDbContext context, OgmaConfig config) : PageModel
{
	public required List<Rating> Ratings { get; set; }
	public required List<StoryCard> Stories { get; set; }
	public required long[] Tags { get; set; }
	public required EStorySortingOptions SortBy { get; set; }
	public required string? SearchBy { get; set; }
	public required long? Rating { get; set; }
	public required Pagination Pagination { get; set; }
	public string PreselectedTagsJson => JsonSerializer.Serialize(Tags, PreselectedTagsJsonContext.Default.Int64Array);

	public record QueryData(
		long[] Tags,
		string? Query = null,
		EStorySortingOptions Sort = EStorySortingOptions.DateDescending,
		long? Rating = null,
		int Page = 1);
	
	public async Task OnGetAsync([FromQuery] QueryData query)
	{
		var (tags, q, sort, rating, page) = query;
		
		var uid = User.GetNumericId();

		SearchBy = q;
		SortBy = sort;
		Rating = rating;
		Tags = tags;

		// Load ratings
		Ratings = await context.Ratings.ToListAsync();

		// Load stories
		var storiesQuery = context.Stories
			.AsQueryable()
			.Search(tags, q, rating)
			.Where(s => s.PublicationDate != null)
			.Where(s => s.ContentBlockId == null)
			.Blacklist(context, uid);

		Stories = await storiesQuery
			.SortByEnum(sort)
			.Paginate(page, config.StoriesPerPage)
			.ProjectToCard()
			.ToListAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = config.StoriesPerPage,
			ItemCount = await storiesQuery.CountAsync(),
			CurrentPage = page,
		};
	}
}

[JsonSerializable(typeof(long[]))]
public partial class PreselectedTagsJsonContext : JsonSerializerContext;