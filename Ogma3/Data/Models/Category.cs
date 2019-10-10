#nullable enable

using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(CTConfig.Category.MaxNameLength)]
        [MinLength(CTConfig.Category.MinNameLength)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(CTConfig.Category.MaxDescLength)]
        [MinLength(CTConfig.Category.MinDescLength)]
        public string Description { get; set; }
    }
}