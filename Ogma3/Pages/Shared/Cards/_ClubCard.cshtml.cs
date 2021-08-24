using AutoMapper;

namespace Ogma3.Pages.Shared.Cards
{
    public record ClubCard
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Slug { get; init; }
        public string Hook { get; init; }
        public string Icon { get; init; }
        public int ThreadsCount { get; init; }
        public int ClubMembersCount { get; init; }
        public int StoriesCount { get; init; }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Data.Clubs.Club, ClubCard>();
        }
    }
}