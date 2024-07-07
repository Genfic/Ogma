#nullable disable

using AutoMapper;

namespace Ogma3.Data.Tags;

public class TagDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public required string Description { get; init; }
	public required ETagNamespace? Namespace { get; init; }
	public string NamespaceColor => Namespace.GetColor();
	public uint? NamespaceId => (uint?)Namespace;

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Tag, TagDto>();
	}
}