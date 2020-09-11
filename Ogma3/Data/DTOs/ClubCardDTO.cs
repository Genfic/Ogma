namespace Ogma3.Data.DTOs
{
    public class ClubCardDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Hook { get; set; }
        public string Icon { get; set; }
        public int UserCount { get; set; }
        public int ThreadCount { get; set; }
        public int StoryCount { get; set; }
    }
}