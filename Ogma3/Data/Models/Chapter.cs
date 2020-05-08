using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Chapter : BaseModel
    {
        [Required]
        public int Order { get; set; }

        [Required]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required]
        [DefaultValue(false)]
        public bool IsPublished { get; set; }

        [Required]
        [MinLength(CTConfig.Chapter.MinTitleLength)]
        [MaxLength(CTConfig.Chapter.MaxTitleLength)]
        public string Title { get; set; }

        [Required] 
        public string Slug { get; set; }

        [Required]
        [MinLength(CTConfig.Chapter.MinBodyLength)]
        [MaxLength(CTConfig.Chapter.MaxBodyLength)]
        public string Body { get; set; }

        [MaxLength(CTConfig.Chapter.MaxNotesLength)]
        public string? StartNotes { get; set; }

        [MaxLength(CTConfig.Chapter.MaxNotesLength)]
        public string? EndNotes { get; set; }

        public  CommentsThread CommentsThread { get; set; }
        
        [Required]
        public int StoryId { get; set; }
    }
}