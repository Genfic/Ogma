using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Ogma3.Data.Enums;

namespace Ogma3.Data.Models
{
    public class Story : BaseModel, IBlockableContent, IReportableContent
    {
        public OgmaUser Author { get; set; }
        public long AuthorId { get; set; }

        [MinLength(CTConfig.CStory.MinTitleLength)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [MinLength(CTConfig.CStory.MinDescriptionLength)]
        public string Description { get; set; }

        [MinLength(CTConfig.CStory.MinHookLength)]
        public string Hook { get; set; }

        public string? Cover { get; set; }
        public string? CoverId { get; set; }

        public DateTime ReleaseDate { get; set; }
        
        public bool IsPublished { get; set; }
        
        // Chapters
        public  ICollection<Chapter> Chapters { get; set; }

        // Tags
        [JsonIgnore]
        public  ICollection<StoryTag> StoryTags { get; set; }

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
        public Rating Rating { get; set; }
        public long RatingId { get; set; }
        
        // Status
        public EStoryStatus Status { get; set; }
        
        // Votes
        public IList<Vote> Votes { get; set; }

        public int WordCount { get; set; }

        public int ChapterCount { get; set; }

        // Just for relationship purposes
        [JsonIgnore]
        public ICollection<Folder> Folders { get; set; }
        
        public ContentBlock? ContentBlock { get; set; }
        public long? ContentBlockId { get; set; }
        
        public ICollection<Report> Reports { get; set; }
    }
}