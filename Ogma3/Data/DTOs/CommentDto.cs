using System;

namespace Ogma3.Data.DTOs
{
    public class CommentDto
    {
        public long Id { get; set; }
        public long CommentsThreadId { get; set; }
        public UserSimpleDto Author { get; set; }
        public DateTime DateTime { get; set; }
        public string Body { get; set; }
    }
}