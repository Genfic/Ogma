#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class Category
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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