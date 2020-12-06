using System;
using System.Collections.Generic;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Pages.Shared.Details
{
    public class BlogpostDetails
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PublishDate { get; set; }
        public string Body { get; set; }
        public IEnumerable<string> Hashtags { get; set; }
        public long CommentsThreadId { get; set; }
        public bool IsPublished { get; set; }
        public int CommentsCount { get; set; }

        // Attachments
        public ChapterMinimal? AttachedChapter { get; set; }
        public StoryMinimal? AttachedStory { get; set; }
        public bool IsUnavailable { get; set; }
        public long? ContentBlockId { get; set; }
    }
}