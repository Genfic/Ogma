using AutoDbSetGenerators;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blacklists;

[AutoDbSet]
public sealed class BlacklistedTag
{
	public OgmaUser User { get; init; } = null!;
	public long UserId { get; init; }
	public Tag Tag { get; init; } = null!;
	public long TagId { get; init; }
}