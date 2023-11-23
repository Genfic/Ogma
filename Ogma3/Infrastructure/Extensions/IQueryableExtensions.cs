using System.Collections.Generic;
using System.Linq;
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
		ICollection<long>? tags = null,
		string? searchQuery = null,
		long? ratingId = null
	)
	{
		// Prepare search query
		var query = stories;

		// Search by title
		if (!string.IsNullOrEmpty(searchQuery))
			query = query.Where(s => EF.Functions.Like(s.Title.ToUpper(), $"%{searchQuery.Trim().ToUpper()}%"));

		// Search by rating
		if (ratingId is not null)
			query = query.Where(s => s.Rating.Id == ratingId);

		// Search by tags
		if (tags is { Count: > 0 })
			query = query.Where(s => s.Tags.Any(st => tags.Contains(st.Id)));

		return query;
	}
}