using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Shelves;

public sealed record ShelfDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required bool IsDefault { get; init; }
	public required bool IsPublic { get; init; }
	public required bool IsQuickAdd { get; init; }
	public required bool TrackUpdates { get; init; }
	public string? Color { get; init; }
	public required int StoriesCount { get; init; }
	public required string? IconName { get; init; }
	public required long? IconId { get; init; }
}

[Mapper]
public static partial class ShelfMapper 
{
	public static partial IQueryable<ShelfDto> ProjectToDto(this IQueryable<Shelf> query);
}