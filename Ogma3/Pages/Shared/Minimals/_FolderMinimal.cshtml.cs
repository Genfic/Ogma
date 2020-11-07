namespace Ogma3.Pages.Shared.Minimals
{
    public class FolderMinimal
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int StoriesCount { get; set; }
    }
}