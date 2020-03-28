using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Namespace : BaseModel
    {
        [Required]
        [MinLength(CTConfig.Namespace.MinNameLength)]
        [MaxLength(CTConfig.Namespace.MaxNameLength)]
        public string Name { get; set; }

        [MinLength(7)]
        [MaxLength(7)]
        public string Color { get; set; }
    }
}