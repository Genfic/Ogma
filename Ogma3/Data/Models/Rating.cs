#nullable enable

using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Rating
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MinLength(CTConfig.Rating.MinNameLength)]
        [MaxLength(CTConfig.Rating.MaxNameLength)]
        public string Name { get; set; }
        
        [Required]
        [MinLength(CTConfig.Rating.MinDescriptionLength)]
        [MaxLength(CTConfig.Rating.MaxDescriptionLength)]
        public string Description { get; set; }
        
        public string Icon { get; set; }
        public string IconId { get; set; }
    }
    
}