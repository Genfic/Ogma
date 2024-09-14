using Ogma3.Data.Stories;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Pages.Shared.Minimals;

public sealed class StoryMinimal
{
	public required long Id { get; init; }
	public required string Title { get; init; }
	public required string AuthorUserName { get; init; }
	public required string Slug { get; init; }
	public required DateTime? PublicationDate { get; init; }
}

[Mapper]
public static partial class StoryMapper
{
	public static partial IQueryable<StoryMinimal> ProjectToMinimal(this IQueryable<Story> queryable);
}