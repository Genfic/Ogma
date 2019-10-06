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
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        public string Cover { get; set; }
        public string CoverId { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }
        
        
        [JsonIgnore]
        public virtual ICollection<StoryTag> StoryTags { get; set; }
        [NotMapped]
        public IList<Tag> Tags => StoryTags.Select(st => st.Tag).ToList();

        [Required]
        public Rating Rating { get; set; }
    }
}