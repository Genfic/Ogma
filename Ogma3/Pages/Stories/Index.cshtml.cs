using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Infrastructure.SearchQueryParser;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Pages.Stories;

public sealed class IndexModel(ApplicationDbContext context, OgmaConfig config, IFusionCache cache) : PageModel
{
	public required List<Rating> Ratings { get; set; }
	public required List<StoryCard> Stories { get; set; }
	public required Pagination? Pagination { get; set; }

	[FromQuery]
	public string? Query { get; set; } = null;
	[FromQuery]
	public long? Rating { get; set; } = null;
	[FromQuery]
	public EStorySortingOptions Sort { get; set; } = EStorySortingOptions.DateDescending;
	[FromQuery]
	public EStoryStatus? Status { get; set; } = null;

	public async Task OnGetAsync([FromQuery] int? page)
	{
		// Load ratings
		Ratings = await context.Ratings.ToListAsync();

		var uid = User.GetNumericId();

		var storiesQuery = context.Stories
			.AsQueryable();

		var tokens = SearchQueryParser.Parse(Query);
		storiesQuery = tokens.Aggregate(storiesQuery, (current, token) => token switch
		{
			TitleToken t => current.Where(s => s.Title.Contains(t.Value)),
			AuthorToken t => t.Negated
				? current.Where(s => !s.Author.NormalizedUserName.Contains(t.Value))
				: current.Where(s => s.Author.NormalizedUserName.Contains(t.Value)),
			RatingToken t => t.Negated
				? current.Where(s => s.Rating.Name != t.Value)
				: current.Where(s => s.Rating.Name == t.Value),
			StatusToken t => t.Negated
				? current.Where(s => s.Status != t.Status)
				: current.Where(s => s.Status == t.Status),
			TagToken { Negated: false } t => t.Ns is null
				? current.Where(s => s.Tags.Any(st => st.Name == t.Value))
				: current.Where(s => s.Tags.Any(st => st.Name == t.Value && st.Namespace == t.Ns)),
			TagToken { Negated: true } t => t.Ns is null
				? current.Where(s => s.Tags.All(st => st.Name != t.Value))
				: current.Where(s => !s.Tags.Any(st => st.Name == t.Value && st.Namespace == t.Ns)),
			_ => current,
		});

		storiesQuery = storiesQuery
			.WhereIf(s => s.RatingId == Rating, Rating != null)
			.WhereIf(s => s.Status == Status, Status != null)
			.Where(s => s.PublicationDate != null)
			.Where(s => s.LastUpdatedAt != null)
			.Where(s => s.ContentBlockId == null)
			.Blacklist(context, uid);

		var key = Query + ":" + string.Join(':', [Rating, (long?)Status, (long?)Sort, page, uid]);
		var (stories, count) = await cache.GetOrSetAsync<(List<StoryCard> s, int c)>(key, async ct => {
			var s = await storiesQuery
				.TagWith("Searching for stories")
				.AsSplitQuery()
				.SortByEnum(Sort)
				.Paginate(page ?? 1, config.StoriesPerPage)
				.Select(StoryMapper.MapToCard)
				.ToListAsync(ct);

			var c = await storiesQuery.TagWith("Counting stories").CountAsync(ct);

			return (s, c);
		}, o => o.SetDuration(TimeSpan.FromMinutes(5)));

		Stories = stories;

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = config.StoriesPerPage,
			ItemCount = count,
			CurrentPage = page ?? 1,
		};
	}
}