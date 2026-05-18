using AutoDbSetGenerators;

namespace Ogma3.Data.Users;

[AutoDbSet(Name = "FollowedUsers")]
public sealed class UserFollow
{
	public OgmaUser FollowingUser { get; set; } = null!;
	public long FollowingUserId { get; set; }
	public OgmaUser FollowedUser { get; set; } = null!;
	public long FollowedUserId { get; set; }
}