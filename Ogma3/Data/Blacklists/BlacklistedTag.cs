#nullable disable

using Ogma3.Data.Tags;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blacklists;

public class BlacklistedTag
{
	public OgmaUser User { get; init; }
	public long UserId { get; init; }
	public Tag Tag { get; init; }
	public long TagId { get; init; }
}