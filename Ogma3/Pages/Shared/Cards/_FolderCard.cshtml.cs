using Ogma3.Data.Folders;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Pages.Shared.Cards;

public class FolderCard
{
	public required long Id { get; init; }
	public required long ClubId { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public required string? Description { get; init; }

	public required int StoriesCount { get; init; }
	public required IEnumerable<FolderMinimalDto> ChildFolders { get; init; }
}

[Mapper]
public static partial class FolderMapper
{
	public static partial IQueryable<FolderCard> ProjectToCard(this IQueryable<Folder> queryable);
}