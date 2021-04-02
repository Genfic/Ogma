using System;
using Ogma3.Data.Chapters;

namespace Ogma3.Pages.Shared.Details
{
    public class ChapterDetails
    {
        public long StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string StorySlug { get; set; }
        public long StoryAuthorId { get; set; }
        public string StoryRatingName { get; set; }
        
        public long Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public uint Order { get; set; }
        public DateTime PublishDate { get; set; }
        public string Body { get; set; }
        public string StartNotes { get; set; }
        public string EndNotes { get; set; }
        
        public long CommentsThreadId { get; set; }
        public bool IsPublished { get; set; }
        public long? ContentBlockId { get; set; }

        public ChapterMicroDto? Previous { get; set; }
        public ChapterMicroDto? Next { get; set; }
    }
}