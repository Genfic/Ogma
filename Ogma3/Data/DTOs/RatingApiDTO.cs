using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class RatingApiDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool BlacklistedByDefault { get; set; }

        public static RatingApiDto FromRating(Rating rating)
        {
            return new RatingApiDto
            {
                Id = rating.Id,
                Name = rating.Name,
                Description = rating.Description,
                Icon = rating.Icon,
                BlacklistedByDefault = rating.BlacklistedByDefault
            };
        }
    }
}