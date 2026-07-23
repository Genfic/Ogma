using System.ComponentModel.DataAnnotations.Schema;
using Ogma3.Data.Blogposts;
using Riok.Mapperly.Abstractions;
using Utils.Extensions;

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

	[MapperIgnore]
	public string Excerpt => Body.ReadUntil("<!-- read more -->", StringComparison.OrdinalIgnoreCase).ToString();
}

[Mapper]
public static partial class NewsMapper
{
	public static partial IQueryable<NewsCard> ProjectToNewsCard(this IQueryable<Blogpost> blogposts);
}