namespace Ogma3.Pages.Shared.Cards;

public sealed class BookshelfCard
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string? IconName { get; init; }
	public required string? Color { get; init; }
	public required string Description { get; init; }
	public required int StoriesCount { get; init; }
}