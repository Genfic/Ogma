using System;

namespace Ogma3.Pages.Shared.Cards
{
    public class BlogpostCard
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        public string AuthorUserName { get; set; }
        public string Body { get; set; }
        public int WordCount { get; set; }
        public string[] Hashtags { get; set; }
    }
}