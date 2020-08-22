using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class Story : BaseModel
    {
        // public Story()
        // {
        //     Chapters = new List<Chapter>();
        // }

        [Required]
        public OgmaUser Author { get; set; }

        [Required]
        [MinLength(CTConfig.CStory.MinTitleLength)]
        [MaxLength(CTConfig.CStory.MaxTitleLength)]
        public string Title { get; set; }

        [Required] 
        public string Slug { get; set; }

        [Required]
        [MinLength(CTConfig.CStory.MinDescriptionLength)]
        [MaxLength(CTConfig.CStory.MaxDescriptionLength)]
        public string Description { get; set; }

        [Required]
        [MinLength(CTConfig.CStory.MinHookLength)]
        [MaxLength(CTConfig.CStory.MaxHookLength)]
        public string Hook { get; set; }

        public string? Cover { get; set; }
        public string? CoverId { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required]
        [DefaultValue(false)]
        public bool IsPublished { get; set; }
        
        // Chapters
        public  ICollection<Chapter> Chapters { get; set; } //= new List<Chapter>();

        // Tags
        [JsonIgnore]
        public  ICollection<StoryTag> StoryTags { get; set; } //= new List<StoryTag>();

        [NotMapped]
        public IEnumerable<Tag> Tags
        {
            get => StoryTags == null || StoryTags.Count <= 0
                    ? new List<Tag>()
                    : StoryTags.Select(st => st.Tag).ToList();
            set => StoryTags = value.Select(t => new StoryTag
            {
                Tag = t,
                TagId = t.Id
            }).ToList();
        }

        // Rating
        [Required]
        public Rating Rating { get; set; }
        
        // Status
        [Required]
        [DefaultValue(EStoryStatus.InProgress)]
        public EStoryStatus Status { get; set; }
        
        // Votes
        public IList<Vote> Votes { get; set; }

        [Required]
        [DefaultValue(0)]
        public int WordCount { get; set; }

        [Required]
        [DefaultValue(0)]
        public int ChapterCount { get; set; }
    
        [NotMapped]
        public int Score => Votes.Count;
    }
}