using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Utils.Extensions;

namespace Ogma3.Data.Models
{
    public class Tag : BaseModel
    {
        private string _name;
        
        [Required]
        [MinLength(CTConfig.CTag.MinNameLength)]
        [MaxLength(CTConfig.CTag.MaxNameLength)]
        public string Name { 
            get => _name;
            set
            {
                _name = value;
                Slug = value.Friendlify();
            } 
        }

        [Required]
        [MinLength(CTConfig.CTag.MinNameLength)]
        [MaxLength(CTConfig.CTag.MaxNameLength)]
        public string Slug { get; set; }

        [MaxLength(CTConfig.CTag.MaxDescLength)]
        public string? Description { get; set; } = null;

        [ForeignKey("NamespaceId")]
        [JsonIgnore]
        public Namespace? Namespace { get; set; } = null;
        public long? NamespaceId { get; set; } = null;
    }
}