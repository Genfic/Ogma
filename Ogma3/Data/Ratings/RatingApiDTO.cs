namespace Ogma3.Data.Ratings
{
    public class RatingApiDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Order { get; set; }
        public string Icon { get; set; }
        public bool BlacklistedByDefault { get; set; }
    }
}