using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Chapters;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Reports;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Blogposts
{
    public class Blogpost : BaseModel, IBlockableContent, IReportableContent
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        public OgmaUser Author { get; set; }
        public long AuthorId { get; set; }
        public string Body { get; set; }
        public CommentsThread CommentsThread { get; set; }
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