using Ogma3.Data.Tags;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blacklists
{
    public class BlacklistedTag
    {
        public OgmaUser User { get; set; }
        public long UserId { get; set; }
        public Tag Tag { get; set; }
        public long TagId { get; set; }
    }
}