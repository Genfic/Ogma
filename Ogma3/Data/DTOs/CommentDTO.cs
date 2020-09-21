using System;
using System.Linq;
using Markdig;
using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class CommentDTO
    {
        public long Id { get; set; }

        public long CommentsThreadId { get; set; }

        public UserSimpleDto Author { get; set; }

        public DateTime DateTime { get; set; }

        public string Body { get; set; }

        public CommentDTO(Comment comment, bool parseMd = false)
        {
            Id = comment.Id;
            
            CommentsThreadId = comment.CommentsThreadId;
            
            DateTime = comment.DateTime;
            
            Body = parseMd ? Markdown.ToHtml(comment.Body.Trim()) : comment.Body.Trim();
            
            var topRole = comment.Author.Roles
                .Where(r => r.Order.HasValue)
                .OrderBy(r => r.Order)
                .First();
            
            var roleDto = new RoleDTO
            {
                Id = topRole.Id,
                Name = topRole.Name,
                Color = topRole.Color,
                IsStaff = topRole.IsStaff
            };
            
            Author = new UserSimpleDto
            {
                UserName = comment.Author.UserName,
                Avatar = comment.Author.Avatar,
                Title = comment.Author.Title,
                TopRole = roleDto
            };
        }
    }
}