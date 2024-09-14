namespace Ogma3.Pages.Shared;

public sealed class Pagination
{
	public int ItemCount { get; init; }
	public int CurrentPage { get; init; }
	public int PerPage { get; init; }
}