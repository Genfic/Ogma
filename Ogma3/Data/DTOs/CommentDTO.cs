using System;
using Markdig;
using Microsoft.Extensions.Configuration;
using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class CommentDTO
    {
        public long Id { get; set; }

        public long CommentsThreadId { get; set; }

        public UserSimpleDTO Author { get; set; }

        public DateTime DateTime { get; set; }

        public string Body { get; set; }

        public CommentDTO(IConfiguration config, Comment comment, bool parseMd = false)
        {
            Id = comment.Id;
            CommentsThreadId = comment.CommentsThreadId;
            DateTime = comment.DateTime;
            Body = parseMd ? Markdown.ToHtml(comment.Body.Trim()) : comment.Body.Trim();
            Author = new UserSimpleDTO(config, comment.Author);
        }
    }
}