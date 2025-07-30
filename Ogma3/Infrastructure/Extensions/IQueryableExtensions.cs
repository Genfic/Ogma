using System.Linq.Expressions;

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
	)
		=> condition ? query.Where(predicate) : query;
}