using System;
using System.Linq;
using Markdig;
using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class CommentDto
    {
        public long Id { get; set; }

        public long CommentsThreadId { get; set; }

        public UserSimpleDto Author { get; set; }

        public DateTime DateTime { get; set; }

        public string Body { get; set; }

        public static CommentDto FromComment(Comment comment, bool parseMd = false)
        {
            var c = new CommentDto
            {
                Id = comment.Id,
                CommentsThreadId = comment.CommentsThreadId,
                DateTime = comment.DateTime,
                Body = parseMd ? Markdown.ToHtml(comment.Body.Trim()) : comment.Body.Trim()
            };
            
            var topRole = comment.Author.Roles
                .Where(r => r.Order.HasValue)
                .OrderBy(r => r.Order)
                .FirstOrDefault();
            
            var roleDtos = comment.Author.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Color = r.Color,
                    IsStaff = r.IsStaff
                });
            
            c.Author = new UserSimpleDto
            {
                UserName = comment.Author.UserName,
                Avatar = comment.Author.Avatar,
                Title = comment.Author.Title,
                Roles = roleDtos
            };

            return c;
        }
    }
}