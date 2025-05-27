using System.ComponentModel;

namespace Ogma3.Infrastructure;

/// <summary>
/// Object to hold the results of a paginated fetch
/// </summary>
/// <typeparam name="T">Type of objects to hold</typeparam>
public sealed class PaginationResult<T>
{
	/// <summary>
	/// List of the fetched elements
	/// </summary>
	[Description("List of the fetched elements")]
	public required List<T> Elements { get; init; }

	/// <summary>
	/// Total number of elements
	/// </summary>
	[Description("Total number of elements")]
	public required int Total { get; init; }

	/// <summary>
	/// Number of elements per page
	/// </summary>
	[Description("Number of elements per page")]
	public required int PerPage { get; init; }

	/// <summary>
	/// Number of pages the total number of elements can be divided into
	/// </summary>
	[Description("Number of pages the total number of elements can be divided into")]
	public required int Pages { get; init; }

	/// <summary>
	/// The requested page
	/// </summary>
	[Description("The requested page")]
	public required int Page { get; init; }
}