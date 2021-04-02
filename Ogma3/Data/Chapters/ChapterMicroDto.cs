namespace Ogma3.Data.Chapters
{
    public class ChapterMicroDto
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public string Slug { get; init; }

        public uint Order { get; set; }
    }
}