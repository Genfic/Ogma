using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Tags;

public sealed class TagDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public required string? Description { get; init; }
	public ETagNamespace? Namespace { get; init; }
	
	[MapperIgnore]
	public string? NamespaceColor => Namespace is null ? null : Namespace.GetColor();
	
	[MapperIgnore]
	public uint? NamespaceId => (uint?)Namespace;
}

[Mapper]
public static partial class TagMapper
{
	public static partial TagDto ToDto(this Tag tag);
	public static partial IQueryable<TagDto> ProjectToDto(this IQueryable<Tag> q);
}