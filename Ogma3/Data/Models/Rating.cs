using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Rating
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        public string Icon { get; set; }
        public string IconId { get; set; }
    }
    
}