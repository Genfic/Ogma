using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;
using Utils.Extensions;
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

		var grouped = Query?.Split(' ').GroupBy(s => {
			if (s.StartsWith('#')) return QueryPartType.Tag;
			if (s.StartsWith("by:")) return QueryPartType.Author;
			return QueryPartType.Query;
		}).ToArray();

		var tags = grouped?.GetValues(k => k == QueryPartType.Tag)
			.Select(t => t.ToLowerInvariant().Trim('#'))
			.ToHashSet();

		var query = string.Join(' ', grouped?.GetValues(k => k == QueryPartType.Query).ToArray() ?? []).Trim();

		var author = grouped?.GetValues(k => k == QueryPartType.Author).FirstOrDefault()?.Replace("by:", "").Trim().Replace('_', ' ');

		var tagIds = tags is { Count: > 0 }
			? await context.Tags
				.TagWith("Searching for tags")
				.Where(t => tags.Contains(t.Name.ToLower()))
				.Select(t => t.Id)
				.ToListAsync()
			: [];

		var key = Query + ":" + string.Join(':', [Rating, (long?)Status, (long?)Sort, page]);
		var (stories, count) = await cache.GetOrSetAsync<(List<StoryCard> s, int c)>(key, async ct => {
			var storiesQuery = context.Stories
				.AsQueryable()
				.WhereIf(s => EF.Functions.ILike(s.Title, $"%{query}%"), !string.IsNullOrEmpty(query))
				.WhereIf(s => s.Rating.Id == Rating, Rating is not null)
				.WhereIf(s => s.Tags.Any(st => tagIds.Contains(st.Id)), tagIds is { Count: > 0})
				.WhereIf(s => s.Status == Status, Status is not null)
				.WhereIf(s => s.Author.NormalizedUserName == author, author is not null)
				.Where(s => s.PublicationDate != null)
				.Where(s => s.ContentBlockId == null)
				.Blacklist(context, uid);

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

	private enum QueryPartType
	{
		Tag,
		Query,
		Author,
	}
}