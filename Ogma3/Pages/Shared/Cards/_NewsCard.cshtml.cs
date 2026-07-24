namespace Ogma3.Pages.Shared.Cards;

public sealed class NewsCard
{
	public required long Id { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required DateTimeOffset? PublicationDate { get; init; }
	public required string AuthorUserName { get; init; }
	public required string AuthorAvatarUrl { get; init; }
	public required string Body { get; init; }
}