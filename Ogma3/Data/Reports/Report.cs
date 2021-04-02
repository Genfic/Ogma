using System;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;

namespace Ogma3.Data.Reports
{
    public class Report : BaseModel
    {
        public OgmaUser Reporter { get; set; }
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