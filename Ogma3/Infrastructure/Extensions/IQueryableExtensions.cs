using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data.Stories;

namespace Ogma3.Infrastructure.Extensions;

public static class QueryableExtensions
{
	/// <summary>
	/// Apply a filter on `IQueryable`
	/// </summary>
	/// <param name="stories">Source IQueryable</param>
	/// <param name="tags">Tags to search by</param>
	/// <param name="searchQuery">Query to search the titles by</param>
	/// <param name="ratingId">Rating to filter by</param>
	/// <returns>`IQueryable` objects with applied filters</returns>
	public static IQueryable<Story> Search(
		this IQueryable<Story> stories,
		List<long>? tags = null,
		string? searchQuery = null,
		long? ratingId = null,
		EStoryStatus? status = null
	)
	{
		// Prepare the search query
		var query = stories;

		// Search by title
		if (!string.IsNullOrEmpty(searchQuery))
		{
			query = query.Where(s => EF.Functions.ILike(s.Title, $"%{searchQuery.Trim()}%"));
		}

		// Search by rating
		if (ratingId is not null)
		{
			query = query.Where(s => s.Rating.Id == ratingId);
		}

		// Search by tags
		if (tags is [_, ..])
		{
			query = query.Where(s => s.Tags.Any(st => tags.Contains(st.Id)));
		}

		if (status is {} ess)
		{
			query = query.Where(s => s.Status == ess);
		}

		return query;
	}

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
	)
		=> condition ? query.Where(predicate) : query;
}