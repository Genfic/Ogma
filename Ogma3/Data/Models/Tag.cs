using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Utils;

namespace Ogma3.Data.Models
{
    public class Tag : BaseModel
    {
        private string _name;
        
        [Required]
        [MinLength(CTConfig.Tag.MinNameLength)]
        [MaxLength(CTConfig.Tag.MaxNameLength)]
        public string Name { 
            get => _name;
            set
            {
                _name = value;
                Slug = value.Friendlify();
            } 
        }

        [Required]
        [MinLength(CTConfig.Tag.MinNameLength)]
        [MaxLength(CTConfig.Tag.MaxNameLength)]
        public string Slug { get; set; }

        [MaxLength(CTConfig.Tag.MaxDescLength)]
        public string? Description { get; set; } = null;

        [ForeignKey("NamespaceId")]
        [JsonIgnore]
        public Namespace? Namespace { get; set; } = null;
        public long? NamespaceId { get; set; } = null;
    }
}