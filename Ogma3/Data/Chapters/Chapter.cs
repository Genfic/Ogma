using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Reports;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Chapters
{
    public class Chapter : BaseModel, IBlockableContent, IReportableContent
    {
        public uint Order { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
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