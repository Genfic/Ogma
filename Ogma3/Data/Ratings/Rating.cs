using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;

namespace Ogma3.Data.Ratings
{
    public class Rating : BaseModel
    {
        [MinLength(CTConfig.CRating.MinNameLength)]
        public string Name { get; set; }
        
        [MinLength(CTConfig.CRating.MinDescriptionLength)]
        public string Description { get; set; }

        public byte Order { get; set; }
        public string Icon { get; set; }
        public string IconId { get; set; }
        public bool BlacklistedByDefault { get; set; }
    }
    
}