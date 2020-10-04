using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Blogpost : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CBlogpost.MinTitleLength)]
        [MaxLength(CTConfig.CBlogpost.MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        public string Slug { get; set; }
        
        [Required]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required] 
        [DefaultValue(false)]
        public bool IsPublished { get; set; }
        
        [Required]
        public OgmaUser Author { get; set; }

        public long AuthorId { get; set; }
        
        [Required]
        [MinLength(CTConfig.CBlogpost.MinBodyLength)]
        [MaxLength(CTConfig.CBlogpost.MaxBodyLength)]
        public string Body { get; set; }

        public  CommentsThread CommentsThread { get; set; }
        
        [Required]
        [DefaultValue(0)]
        public int WordCount { get; set; }
        
        [MaxLength(CTConfig.CBlogpost.MaxTagsAmount)]
        [DefaultValue(new string[0])]
        [Required]
        public string[] Hashtags { get; set; } = Array.Empty<string>();
    }
}