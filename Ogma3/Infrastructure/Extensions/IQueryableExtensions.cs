using System.Linq.Expressions;
using Ogma3.Data;
using Ogma3.Data.Stories;

namespace Ogma3.Infrastructure.Extensions;

public static class QueryableExtensions
{
	/// <summary>
	/// Filters a sequence based on a predicate, if a condition is met
	/// </summary>
	/// <param name="query">An <see cref="IQueryable{T}"/> to filter</param>
	/// <param name="predicate">A function to test each element for a condition</param>
	/// <param name="condition">A condition for filtering the sequence</param>
	/// <typeparam name="TSource">The type of the elements of the source</typeparam>
	/// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the condition specified by predicate</returns>
	public static IQueryable<TSource> WhereIf<TSource>(
		this IQueryable<TSource> query,
		Expression<Func<TSource, bool>> predicate,
		bool condition
	) => condition ? query.Where(predicate) : query;

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
		{
			throw new ArgumentOutOfRangeException(nameof(page), "Page has to be greater than 0");
		}
		if (perPage < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage has to be greater than 0");
		}

		return query
			.Skip(Math.Max(0, page - 1) * perPage)
			.Take(perPage);
	}

	extension(IQueryable<Story> query)
	{
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