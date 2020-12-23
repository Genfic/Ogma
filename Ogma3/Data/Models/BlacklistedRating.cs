using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class BlacklistedRating
    {
        [Required]
        public OgmaUser User { get; set; }
        public long UserId { get; set; }

        [Required]
        public Rating Rating { get; set; }
        public long RatingId { get; set; }
    }
}