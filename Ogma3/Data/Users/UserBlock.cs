using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Users
{
    public class UserBlock
    {
        [Required]
        public OgmaUser BlockingUser { get; set; }
        public long BlockingUserId { get; set; }

        [Required]
        public OgmaUser BlockedUser { get; set; }
        public long BlockedUserId { get; set; }
    }
}