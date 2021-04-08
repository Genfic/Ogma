using System;

namespace Ogma3.Data.Blogposts
{
    public class BlogpostSimpleDto
    {
        public long Id { get; init; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        public int CommentsThreadCommentsCount { get; set; }
    }
}