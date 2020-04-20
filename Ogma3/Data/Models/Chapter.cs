using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class Chapter
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        public  CommentsThread CommentsThread { get; set; } //= new CommentsThread();
        public int CommentsThreadId { get; set; }
        
        [Required]
        public int StoryId { get; set; }
    }
}