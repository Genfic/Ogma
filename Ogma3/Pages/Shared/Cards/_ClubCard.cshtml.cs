namespace Ogma3.Pages.Shared.Cards;

public record ClubCard
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public required string Hook { get; init; }
	public required string Icon { get; init; }
	public required int ThreadsCount { get; init; }
	public required int ClubMembersCount { get; init; }
	public required int StoriesCount { get; init; }
}