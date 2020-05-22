using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class Rating : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CRating.MinNameLength)]
        [MaxLength(CTConfig.CRating.MaxNameLength)]
        public string Name { get; set; }
        
        [Required]
        [MinLength(CTConfig.CRating.MinDescriptionLength)]
        [MaxLength(CTConfig.CRating.MaxDescriptionLength)]
        public string Description { get; set; }
        
        public string Icon { get; set; }
        public string IconId { get; set; }
    }
    
}