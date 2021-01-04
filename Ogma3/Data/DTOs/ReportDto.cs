using System;

namespace Ogma3.Data.DTOs
{
    public class ReportDto
    {
        public long Id { get; set; }
        
        public string ReporterUserName { get; set; }
        public long ReporterId { get; set; }

        public DateTime ReportDate { get; set; }

        public string Reason { get; set; }

        // Blockable content
        public string ContentType { get; set; }
        
        public long? CommentId { get; set; }
        public long? CommentCommentsThreadId { get; set; }

        public string? UserUserName { get; set; }
        public long? UserId { get; set; }

        public long? StoryId { get; set; }

        public long? ChapterId { get; set; }

        public long? BlogpostId { get; set; }

        public string? ClubName { get; set; }
        public long? ClubId { get; set; }
    }
}