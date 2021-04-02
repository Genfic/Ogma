using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Users
{
    public class UserFollow
    {
        [Required]
        public OgmaUser FollowingUser { get; set; }
        public long FollowingUserId { get; set; }
        
        [Required]
        public OgmaUser FollowedUser { get; set; }
        public long FollowedUserId { get; set; }
    }
}