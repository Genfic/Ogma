using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Tags;

[AutoDbSet]
public sealed class Tag : BaseModel
{
	public string Name { get; init; } = null!;
	public string Slug { get; init; } = null!;
	public string? Description { get; init; }
	public ETagNamespace? Namespace { get; init; }
	public List<Story> Stories { get; init; } = null!;

	public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
	public OgmaUser? CreatedBy { get; set; }
	public long? CreatedById { get; set; }
}