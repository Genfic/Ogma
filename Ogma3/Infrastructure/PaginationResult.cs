namespace Ogma3.Infrastructure;

/// <summary>
/// Object to hold the results of a paginated fetch
/// </summary>
/// <typeparam name="T">Type of objects to hold</typeparam>
public class PaginationResult<T>
{
	/// <summary>
	/// List of the fetched elements
	/// </summary>
	public required IEnumerable<T> Elements { get; init; }

	/// <summary>
	/// Total number of those elements, not just the number of them on the page
	/// </summary>
	public required int Total { get; init; }

	/// <summary>
	/// Number of elements per page
	/// </summary>
	public required int PerPage { get; init; }

	/// <summary>
	/// Number of pages the total number of elements can be divided into
	/// </summary>
	public required int Pages { get; init; }

	/// <summary>
	/// Number of the requested page
	/// </summary>
	public required int Page { get; init; }
}