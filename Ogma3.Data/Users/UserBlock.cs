using AutoDbSetGenerators;

namespace Ogma3.Data.Users;

[AutoDbSet(Name = "BlockedUsers")]
public sealed class UserBlock
{
	public OgmaUser BlockingUser { get; set; } = null!;
	public long BlockingUserId { get; set; }
	public OgmaUser BlockedUser { get; set; } = null!;
	public long BlockedUserId { get; set; }
}