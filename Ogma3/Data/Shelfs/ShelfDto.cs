namespace Ogma3.Data.Shelfs
{
    public class ShelfDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsDefault { get; init; }
        public bool IsPublic { get; init; }
        public bool IsQuick { get; init; }
        public bool TrackUpdates { get; init; }
        public string Color { get; init; }
        public int StoriesCount { get; init; }
        public string? IconName { get; init; }
        public long? IconId { get; init; }
        public bool? DoesContainBook { get; init; }
    }
}