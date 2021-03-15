using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public  IList<Chapter> Chapters { get; set; }

        // Tags
        public IEnumerable<Tag> Tags { get; set; }

        // Rating
        public Rating Rating { get; set; }
        public long RatingId { get; set; }
        
        // Status
        public EStoryStatus Status { get; set; }
        
        // Votes
        public ICollection<Vote> Votes { get; set; }

        public int WordCount { get; set; }

        public int ChapterCount { get; set; }

        // Just for relationship purposes
        [JsonIgnore]
        public ICollection<Folder> Folders { get; set; }
        
        public ContentBlock? ContentBlock { get; set; }
        public long? ContentBlockId { get; set; }
        
        public ICollection<Report> Reports { get; set; }
        public ICollection<Shelf> Shelves { get; set; }
    }
}