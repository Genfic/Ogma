#nullable enable

using System;
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

        public DateTime PublishDate { get; set; }

        [Required]
        public bool IsPublished { get; set; } = false;

        [Required]
        [MinLength(CTConfig.Chapter.MinTitleLength)]
        [MaxLength(CTConfig.Chapter.MaxTitleLength)]
        public string Title { get; set; }

        [Required] 
        public string Slug { get; set; } = "";

        [Required]
        [MinLength(CTConfig.Chapter.MinBodyLength)]
        [MaxLength(CTConfig.Chapter.MaxBodyLength)]
        public string Body { get; set; }

        [MaxLength(CTConfig.Chapter.MaxNotesLength)]
        public string? StartNotes { get; set; } = null;

        [MaxLength(CTConfig.Chapter.MaxNotesLength)]
        public string? EndNotes { get; set; } = null;

        public CommentsThread? CommentsThread { get; set; }
        public int? CommentsThreadId { get; set; } /* TODO: Fix the relationship somehow */
        
        [Required]
        public int StoryId { get; set; }
    }
}