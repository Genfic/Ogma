#nullable disable

using AutoDbSetGenerators;

namespace Ogma3.Data.Users;

[AutoDbSet(Name = "BlockedUsers")]
public sealed class UserBlock
{
	public OgmaUser BlockingUser { get; set; }
	public long BlockingUserId { get; set; }
	public OgmaUser BlockedUser { get; set; }
	public long BlockedUserId { get; set; }
}