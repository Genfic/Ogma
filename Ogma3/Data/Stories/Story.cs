using System;
using System.Collections.Generic;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Chapters;
using Ogma3.Data.Folders;
using Ogma3.Data.Ratings;
using Ogma3.Data.Reports;
using Ogma3.Data.Shelves;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;
using Ogma3.Data.Votes;

namespace Ogma3.Data.Stories
{
    public class Story : BaseModel, IBlockableContent, IReportableContent
    {
        public OgmaUser Author { get; set; }
        public long AuthorId { get; set; }
        public string Title { get; set; }

        public string Slug { get; set; }
        public string Description { get; set; }
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
        public ICollection<Folder> Folders { get; set; }
        
        public ContentBlock? ContentBlock { get; set; }
        public long? ContentBlockId { get; set; }
        
        public ICollection<Report> Reports { get; set; }
        public ICollection<Shelf> Shelves { get; set; }
    }
}