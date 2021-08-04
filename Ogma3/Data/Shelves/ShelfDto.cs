using AutoMapper;

namespace Ogma3.Data.Shelves
{
    public record ShelfDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsDefault { get; init; }
        public bool IsPublic { get; init; }
        public bool IsQuickAdd { get; init; }
        public bool TrackUpdates { get; init; }
        public string Color { get; init; }
        public int StoriesCount { get; init; }
        public string? IconName { get; init; }
        public long? IconId { get; init; }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Shelf, ShelfDto>();
        }
    }
}