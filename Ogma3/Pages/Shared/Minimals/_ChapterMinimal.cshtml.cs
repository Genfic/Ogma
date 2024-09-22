using Ogma3.Data.Chapters;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Pages.Shared.Minimals;

public sealed class ChapterMinimal
{
	public required long Id { get; init; }
	public required string StoryTitle { get; init; }
	public required string StoryAuthorUserName { get; init; }
	public required long StoryId { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public required DateTimeOffset? PublicationDate { get; init; }
}

[Mapper]
public static partial class ChapterMapper
{
	public static partial IQueryable<ChapterMinimal> ProjectToMinimal(this IQueryable<Chapter> queryable);
}