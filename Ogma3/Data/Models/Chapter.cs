using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Chapter : BaseModel, IBlockableContent, IReportableContent
    {
        public uint Order { get; set; }
        public DateTime PublishDate { get; set; }

        public bool IsPublished { get; set; }

        [MinLength(CTConfig.CChapter.MinTitleLength)]
        public string Title { get; set; }
        
        public string Slug { get; set; }

        [MinLength(CTConfig.CChapter.MinBodyLength)]
        public string Body { get; set; }

        public string? StartNotes { get; set; }

        public string? EndNotes { get; set; }

        public int WordCount { get; set; }

        public  CommentsThread CommentsThread { get; set; }
        
        public Story Story { get; set; }
        public long StoryId { get; set; }
        
        public ContentBlock? ContentBlock { get; set; }
        public long? ContentBlockId { get; set; }
        
        public ICollection<Report> Reports { get; set; }
    }
}