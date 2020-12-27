using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Blogpost : BaseModel, IBlockableContent, IReportableContent
    {
        [MinLength(CTConfig.CBlogpost.MinTitleLength)]
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PublishDate { get; set; }

        public bool IsPublished { get; set; }
        
        public OgmaUser Author { get; set; }
        public long AuthorId { get; set; }
        
        [MinLength(CTConfig.CBlogpost.MinBodyLength)]
        public string Body { get; set; }

        public CommentsThread CommentsThread { get; set; }
        public long CommentsThreadId { get; set; }
        
        public int WordCount { get; set; }
        
        public string[] Hashtags { get; set; }

        // Attachments
        public Story? AttachedStory { get; set; }
        public long? AttachedStoryId { get; set; }
        
        public Chapter? AttachedChapter { get; set; }
        public long? AttachedChapterId { get; set; }

        public ContentBlock? ContentBlock { get; set; }
        public long? ContentBlockId { get; set; }
        
        public ICollection<Report> Reports { get; set; }
    }
}