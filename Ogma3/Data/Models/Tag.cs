#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ogma3.Data.Models
{
    public class Tag : BaseModel
    {
        [Required]
        [MinLength(CTConfig.Tag.MinNameLength)]
        [MaxLength(CTConfig.Tag.MaxNameLength)]
        public string Name { get; set; }

        [MaxLength(CTConfig.Tag.MaxDescLength)]
        public string? Description { get; set; } = null;

        [ForeignKey("NamespaceId")]
        [JsonIgnore]
        public  Namespace? Namespace { get; set; } = null;
        public int? NamespaceId { get; set; } = null;
    }
}