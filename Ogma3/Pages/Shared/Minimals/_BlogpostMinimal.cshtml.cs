using System;

namespace Ogma3.Pages.Shared.Minimals;

public class BlogpostMinimal
{
	public required long Id { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required DateTime PublishDate { get; init; }
}