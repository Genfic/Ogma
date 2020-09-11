using System;

namespace Ogma3.Data.DTOs
{
    public class ClubBarDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Hook { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserCount { get; set; }
        public int ThreadCount { get; set; }
        public int StoryCount { get; set; }
    }
}