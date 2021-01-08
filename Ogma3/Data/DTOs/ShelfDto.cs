namespace Ogma3.Data.DTOs
{
    public class ShelfDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public bool IsQuick { get; set; }
        public string Color { get; set; }
        public int StoriesCount { get; set; }
        public string? IconName { get; set; }
        public long? IconId { get; set; }
        public bool? DoesContainBook { get; set; }
    }
}