using Ogma3.Data.Ratings;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blacklists
{
    public class BlacklistedRating
    {
        public OgmaUser User { get; set; }
        public long UserId { get; set; }
        public Rating Rating { get; set; }
        public long RatingId { get; set; }
    }
}