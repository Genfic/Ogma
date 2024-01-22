namespace Ogma3.Pages.Shared.Minimals;

public class FolderMinimal
{
	public required long Id { get; init; }
	public required long ClubId { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public required int StoriesCount { get; init; }
}