using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.Blogposts
{
    public class BlogpostCreateDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public ChapterMinimal? ChapterMinimal { get; set; }
        public long? ChapterMinimalId { get; set; }
        public StoryMinimal? StoryMinimal { get; set; }
        public long? StoryMinimalId { get; set; }
        public bool IsUnavailable { get; set; }
    }
}