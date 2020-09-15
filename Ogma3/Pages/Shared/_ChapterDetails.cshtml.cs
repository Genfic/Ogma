using System;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Shared
{
    public class ChapterDetails
    {
        public long StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string StorySlug { get; set; }
        public long StoryAuthorId { get; set; }
        
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public string Body { get; set; }
        public string StartNotes { get; set; }
        public string EndNotes { get; set; }
        
        public long CommentsThreadId { get; set; }
    }
}