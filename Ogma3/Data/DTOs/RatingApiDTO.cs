using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class RatingApiDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public static RatingApiDTO FronRating(Rating rating)
        {
            return new RatingApiDTO
            {
                Id = rating.Id,
                Name = rating.Name,
                Description = rating.Description,
                Icon = rating.Icon
            };
        }
    }
}