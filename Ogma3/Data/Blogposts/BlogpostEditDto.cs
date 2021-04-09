using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data.Blogposts
{
    public class BlogpostEditDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public ChapterMinimal? AttachedChapter { get; set; }
        public StoryMinimal? AttachedStory { get; set; }
        public bool IsUnavailable { get; set; }
        public bool Published { get; set; }
    }
}