namespace Ogma3.Pages.Shared.Minimals;

public sealed class BlogpostMinimal
{
	public required long Id { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required DateTimeOffset PublishDate { get; init; }
}