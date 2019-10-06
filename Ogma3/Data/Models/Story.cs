#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ogma3.Data.Models
{
    public class Story
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MinLength(100)]
        [MaxLength(1500)]
        public string Description { get; set; }

        [Required]
        [MinLength(50)]
        [MaxLength(500)]
        public string Hook { get; set; }

        public string? Cover { get; set; }
        public string? CoverId { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }
        
        
        // Chapters
        public ICollection<Chapter> Chapters { get; set; }
        
        
        // Tags
        [JsonIgnore]
        public virtual ICollection<StoryTag> StoryTags { get; set; }
        [NotMapped]
        public IList<Tag> Tags => StoryTags.Select(st => st.Tag).ToList();
        
        // Rating
        [Required]
        public Rating Rating { get; set; }
    }
}