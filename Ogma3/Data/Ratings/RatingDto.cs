using AutoMapper;

namespace Ogma3.Data.Ratings;

public class RatingDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Icon { get; init; }
        
    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<Rating, RatingDto>();
    }
}