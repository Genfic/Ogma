#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ogma3.Data.Models
{
    public class Tag
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        [ForeignKey("NamespaceId")]
        [JsonIgnore]
        public Namespace? Namespace { get; set; }
        public int? NamespaceId { get; set; }
    }
}