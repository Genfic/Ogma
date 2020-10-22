namespace Ogma3.Data.Models
{
    public class BlacklistedRating
    {
        public OgmaUser User { get; set; }
        public long UserId { get; set; }

        public Rating Rating { get; set; }
        public long RatingId { get; set; }
    }
}