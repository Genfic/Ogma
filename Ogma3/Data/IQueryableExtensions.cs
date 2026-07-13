using Ogma3.Data.Stories;

namespace Ogma3.Data;

public static class QueryableExtensions
{
	/// <summary>
	/// Shorthand for `.Skip().Take()`
	/// </summary>
	/// <param name="query">`IQueryable` object</param>
	/// <param name="page">Page number</param>
	/// <param name="perPage">Number of results per page</param>
	/// <typeparam name="T">Type of the object</typeparam>
	/// <exception cref="ArgumentOutOfRangeException">Throw an error where either `page` or `perPage` are less than 1</exception>
	/// <returns>Desired page of the objects</returns>
	public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int perPage)
	{
		if (page < 1)
			throw new ArgumentOutOfRangeException(nameof(page), "Page has to be greater than 0");
		if (perPage < 1)
			throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage has to be greater than 0");

		return query
			.Skip(Math.Max(0, page - 1) * perPage)
			.Take(perPage);
	}

	extension(IQueryable<Story> query)
	{
		public IQueryable<Story> SortByEnum(EStorySortingOptions order)
		{
			return order switch
			{
				EStorySortingOptions.TitleAscending => query.OrderBy(s => s.Title),
				EStorySortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
				EStorySortingOptions.DateAscending => query.OrderBy(s => s.PublicationDate),
				EStorySortingOptions.DateDescending => query.OrderByDescending(s => s.PublicationDate),
				EStorySortingOptions.WordsAscending => query.OrderBy(s => s.WordCount),
				EStorySortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
				EStorySortingOptions.ScoreAscending => query.OrderBy(s => s.VoteCount),
				EStorySortingOptions.ScoreDescending => query.OrderByDescending(s => s.VoteCount),
				EStorySortingOptions.UpdatedAscending => query.OrderBy(s => s.LastUpdatedAt),
				EStorySortingOptions.UpdatedDescending => query.OrderByDescending(s => s.LastUpdatedAt),
				_ => query.OrderByDescending(s => s.CreationDate),
			};
		}

		public IQueryable<Story> Blacklist(ApplicationDbContext ctx, long? userId)
		{
			return userId is not null
				? query
					.Where(s => !ctx.BlacklistedRatings
						.Where(br => br.UserId == userId)
						.Any(br => br.RatingId == s.RatingId)
					)
					.Where(s => !ctx.BlacklistedTags
						.Where(bt => bt.UserId == userId)
						.Any(bt => s.Tags.Any(t => t.Id == bt.TagId))
					)
					.Where(s => !ctx.BlockedUsers
						.Where(bu => bu.BlockingUserId == userId)
						.Any(bu => bu.BlockedUserId == s.AuthorId)
					)
				: query
					.Where(s => !s.Rating.BlacklistedByDefault);
		}
	}

}