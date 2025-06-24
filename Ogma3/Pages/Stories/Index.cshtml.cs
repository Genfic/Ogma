using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;
using ZiggyCreatures.Caching.Fusion;

namespace Ogma3.Pages.Stories;

public sealed class IndexModel(ApplicationDbContext context, OgmaConfig config, IFusionCache cache) : PageModel
{
	public required List<Rating> Ratings { get; set; }
	public required List<StoryCard> Stories { get; set; }
	public required Pagination Pagination { get; set; }

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
		var tags = Query?.Split(' ')
			.Where(s => s.StartsWith('#'))
			.Select(t => t.ToLowerInvariant().Trim('#'))
			.Select(t => t.StartsWith("cw:") ? t.Replace("cw:", "content_warning:") : t)
			.Select(t => t.Contains(':') ? t : $":{t}").ToArray();

		List<long> tagIds = [];
		var query = Query;
		if (tags != null)
		{
			tagIds = await context.Tags
				.TagWith("Searching for tags")
				.Where(t => tags.Contains(t.Namespace + ":" + t.Name.ToLower()))
				.Select(t => t.Id)
				.ToListAsync();

			query = string.Join(' ', Query?.Split(' ').Where(s => !s.StartsWith('#')).ToArray() ?? []).Trim();
		}

		var (stories, count) = await cache.GetOrSetAsync<(List<StoryCard> s, int c)>($"{Query}:{Rating}:{Status?.ToStringFast()}:{Sort.ToStringFast()}:${Pagination.CurrentPage}", async ct => {
			var storiesQuery = context.Stories
				.AsQueryable()
				.Search(tagIds, query, Rating, Status)
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

	private sealed record TagEntry(string Name, string Namespace);
}