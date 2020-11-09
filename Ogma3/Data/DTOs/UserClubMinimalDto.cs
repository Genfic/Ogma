namespace Ogma3.Data.DTOs
{
    public record UserClubMinimalDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Icon { get; init; }
    }
}