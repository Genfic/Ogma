namespace Ogma3.Data.Models
{
    public class BlacklistedUser
    {
        public OgmaUser User { get; set; }
        public long UserId { get; set; }

        public OgmaUser BlockedUser { get; set; }
        public long BlockedUserId { get; set; }
    }
}