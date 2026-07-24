using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Exceptions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.OgmaConfig;
using Ogma3.Infrastructure.SearchQueryParser;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Services.TagCache;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Pages.Stories;

public sealed class IndexModel(
	ApplicationDbContext context,
	OgmaConfig config,
	TagCache tagCache,
	IFusionCache cache) : PageModel
{
	public required List<Rating> Ratings { get; set; }
	public required List<StoryCard> Stories { get; set; }
	public required Pagination? Pagination { get; set; }

	[FromQuery]
	public string? Query { get; set; }
	[FromQuery]
	public long? Rating { get; set; }
	[FromQuery]
	public StorySort? Sort { get; set; }
	[FromQuery]
	public EStoryStatus? Status { get; set; }

	public async Task OnGetAsync([FromQuery] int? page)
	{
		// Load ratings
		Ratings = await context.Ratings.ToListAsync();

		var uid = User.GetNumericId();

		var storiesQuery = context.Stories
			.AsQueryable();

		var tokens = SearchQueryParser.Parse(Query);

		var includedTags = new HashSet<string>();
		var excludedTags = new HashSet<string>();
		foreach (var token in tokens)
		{
			switch (token)
			{
				case TitleToken t:
					storiesQuery = storiesQuery.Where(s => s.Title.Contains(t.Value));
					break;
				case AuthorToken t:
					storiesQuery = t.Negated
						? storiesQuery.Where(s => !s.Author.NormalizedUserName.Contains(t.Value))
						: storiesQuery.Where(s => s.Author.NormalizedUserName.Contains(t.Value));
					break;
				case RatingToken t:
					storiesQuery = t.Negated
						? storiesQuery.Where(s => s.Rating.Name != t.Value)
						: storiesQuery.Where(s => s.Rating.Name == t.Value);
					break;
				case StatusToken t:
					storiesQuery = t.Negated
						? storiesQuery.Where(s => s.Status != t.Status)
						: storiesQuery.Where(s => s.Status == t.Status);
					break;
				case TagToken { Negated: false } t:
					includedTags.Add(t.FullName);
					break;
				case TagToken { Negated: true } t:
					excludedTags.Add(t.FullName);
					break;
			}
		}

		var includedTagIds = await tagCache.GetTagIds([..includedTags]);
		var excludedTagIds = await tagCache.GetTagIds([..excludedTags]);

		storiesQuery = storiesQuery
			.WhereIf(s =>  includedTagIds.All(i => s.Tags.Any(t => t.Id == i)), includedTagIds.Length > 0)
			.WhereIf(s => !s.Tags.Any(t =>  excludedTagIds.AsEnumerable().Contains(t.Id)), excludedTagIds.Length > 0)
			.WhereIf(s => s.RatingId == Rating, Rating != null)
			.WhereIf(s => s.Status == Status, Status != null)
			.Where(s => s.IsVisible)
			.Where(s => s.LastUpdatedAt != null)
			.Where(s => s.ContentBlockId == null)
			.Blacklist(context, uid);

		var key = Query + ":" + string.Join(':', [Rating, (long?)Status, (long?)Sort, page, uid]);
		var (stories, count) = await cache.GetOrSetAsync<(List<StoryCard> s, int c)>(key, async ct => {

			var s = await SortStories(storiesQuery, Sort ?? StorySort.DateDescending)
				.TagWith("Searching for stories")
				.AsSplitQuery()
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

	private static IQueryable<Story> SortStories(IQueryable<Story> query, StorySort sortBy)
		=> sortBy switch
		{
			StorySort.TitleAscending => query.OrderBy(s => s.Title),
			StorySort.TitleDescending => query.OrderByDescending(s => s.Title),
			StorySort.DateAscending => query.OrderBy(s => s.PublicationDate),
			StorySort.DateDescending => query.OrderByDescending(s => s.PublicationDate),
			StorySort.WordsAscending => query.OrderBy(s => s.WordCount),
			StorySort.WordsDescending => query.OrderByDescending(s => s.WordCount),
			StorySort.ScoreAscending => query.OrderBy(s => s.VoteCount),
			StorySort.ScoreDescending => query.OrderByDescending(s => s.VoteCount),
			StorySort.UpdatedAscending => query.OrderBy(s => s.LastUpdatedAt),
			StorySort.UpdatedDescending => query.OrderByDescending(s => s.LastUpdatedAt),
			_ => throw new UnexpectedEnumValueException<StorySort>(sortBy),
		};
}