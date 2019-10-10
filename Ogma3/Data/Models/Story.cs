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
        [MinLength(CTConfig.Story.MinTitleLength)]
        [MaxLength(CTConfig.Story.MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [MinLength(CTConfig.Story.MinDescriptionLength)]
        [MaxLength(CTConfig.Story.MaxDescriptionLength)]
        public string Description { get; set; }

        [Required]
        [MinLength(CTConfig.Story.MinHookLength)]
        [MaxLength(CTConfig.Story.MaxHookLength)]
        public string Hook { get; set; }

        public string? Cover { get; set; } = null;
        public string? CoverId { get; set; } = null;

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