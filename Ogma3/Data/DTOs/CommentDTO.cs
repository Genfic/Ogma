using System;
using Markdig;
using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public int CommentsThreadId { get; set; }

        public UserSimpleDTO Author { get; set; }

        public DateTime DateTime { get; set; }

        public string Body { get; set; }

        public static CommentDTO FromComment(Comment comment, bool parseMd = false)
        {
            var body = parseMd ? Markdown.ToHtml(comment.Body.Trim()) : comment.Body;
            return new CommentDTO
            {
                Id = comment.Id,
                CommentsThreadId = comment.CommentsThreadId,
                DateTime = comment.DateTime,
                Body = body.Trim(),
                Author = UserSimpleDTO.FromUser(comment.Author)
            };
        }
    }
}