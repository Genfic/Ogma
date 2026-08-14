using MemoryPack;
using Ogma3.Infrastructure.Extensions;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Tags;

[MemoryPackable]
public sealed partial class TagDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public required string? Description { get; init; }
	public string? NamespaceName { get; init; }
	public string? NamespaceColor { get; init; }
}

[Mapper]
public static partial class TagMapper
{
	public static partial TagDto ToDto(this Tag tag);
	public static partial IQueryable<TagDto> ProjectToDto(this IQueryable<Tag> q);
}