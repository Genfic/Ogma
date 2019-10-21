using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class Namespace
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MinLength(CTConfig.Namespace.MinNameLength)]
        [MaxLength(CTConfig.Namespace.MaxNameLength)]
        public string Name { get; set; }
    }
}