using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Tags;

namespace Ogma3.Data.TagNamespaces;

[AutoDbSet]
public sealed class TagNamespace : BaseModel
{
	public required string Name { get; init; }
	public required string Slug { get; init; }
	public string? Alias { get; init; }
	public string? Color { get; init; }
	public string? Description { get; init; }

	public List<Tag> Tags { get; init; } = [];
}