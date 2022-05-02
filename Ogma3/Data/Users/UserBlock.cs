namespace Ogma3.Data.Users;

public class UserBlock
{
	public OgmaUser BlockingUser { get; set; }
	public long BlockingUserId { get; set; }
	public OgmaUser BlockedUser { get; set; }
	public long BlockedUserId { get; set; }
}