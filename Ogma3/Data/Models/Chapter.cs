using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Ogma3.Data.Models
{
    public class Chapter : BaseModel, IBlockableContent, IReportableContent
    {
        [Required]
        public uint Order { get; set; }

        [Required]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required]
        [DefaultValue(false)]
        public bool IsPublished { get; set; }

        [Required]
        [MinLength(CTConfig.CChapter.MinTitleLength)]
        [MaxLength(CTConfig.CChapter.MaxTitleLength)]
        public string Title { get; set; }

        [Required] 
        public string Slug { get; set; }

        [Required]
        [MinLength(CTConfig.CChapter.MinBodyLength)]
        [MaxLength(CTConfig.CChapter.MaxBodyLength)]
        public string Body { get; set; }

        [MaxLength(CTConfig.CChapter.MaxNotesLength)]
        public string? StartNotes { get; set; }

        [MaxLength(CTConfig.CChapter.MaxNotesLength)]
        public string? EndNotes { get; set; }

        [Required]
        public int WordCount { get; set; }

        public  CommentsThread CommentsThread { get; set; }
        
        
        [JsonIgnore]
        [Required]
        public Story Story { get; set; }
        public long StoryId { get; set; }
        
        public ContentBlock? ContentBlock { get; set; }
        public long? ContentBlockId { get; set; }
        
        public ICollection<Report> Reports { get; set; }
    }
}