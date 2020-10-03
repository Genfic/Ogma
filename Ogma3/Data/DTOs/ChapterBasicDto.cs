using System;

namespace Ogma3.Data.DTOs
{
    public class ChapterBasicDto
    {
        public long Id { get; set; }
        public uint Order { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public int WordCount { get; set; }
    }
}