using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Namespace : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CNamespace.MinNameLength)]
        [MaxLength(CTConfig.CNamespace.MaxNameLength)]
        public string Name { get; set; }

        [MinLength(7)]
        [MaxLength(7)]
        public string Color { get; set; }
    }
}