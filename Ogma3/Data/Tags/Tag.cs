using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Tags;

[AutoDbSet]
public sealed class Tag : BaseModel
{
	public string Name { get; init; } = null!;
	public string Slug { get; init; } = null!;
	public string? Description { get; init; }
	public ETagNamespace? Namespace { get; init; }
	public IEnumerable<Story> Stories { get; init; } = null!;
}