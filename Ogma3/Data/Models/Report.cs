using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Report : BaseModel
    {
        [Required]
        public OgmaUser Reporter { get; set; }
        [Required]
        public long ReporterId { get; set; }

        [Required]
        public DateTime ReportDate { get; set; } = DateTime.Now;

        [Required]
        public string Reason { get; set; }

        // Blockable content
        [Required]
        public string ContentType { get; set; }
        
        public Comment? Comment { get; set; }
        public long? CommentId { get; set; }

        public OgmaUser? User { get; set; }
        public long? UserId { get; set; }

        public Story? Story { get; set; }
        public long? StoryId { get; set; }

        public Chapter? Chapter { get; set; }
        public long? ChapterId { get; set; }

        public Blogpost? Blogpost { get; set; }
        public long? BlogpostId { get; set; }

        public Club? Club { get; set; }
        public long? ClubId { get; set; }
    }
}