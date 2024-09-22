using Ogma3.Data.Blogposts;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Pages.Shared.Cards;

public sealed class BlogpostCard
{
	public required long Id { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required DateTimeOffset? PublicationDate { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Body { get; init; }
	public required int WordCount { get; init; }
	public required string[] Hashtags { get; init; }
}

[Mapper]
public static partial class BlogpostMapper
{
	public static partial IQueryable<BlogpostCard> ProjectToCard(this IQueryable<Blogpost> blogposts);
	public static partial BlogpostCard ToCard(this Blogpost blogpostCard);
}