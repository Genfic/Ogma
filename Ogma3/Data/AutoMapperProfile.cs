using System.Linq;
using AutoMapper;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared;

namespace Ogma3.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Chapter mappings
            CreateMap<Chapter, ChapterDetails>();
            CreateMap<Chapter, ChapterBasicDto>();

            // Story mappings
            CreateMap<Story, StoryDetails>()
                .ForMember(sd => sd.Tags, opts => opts.MapFrom(s => s.StoryTags.Select(st => st.Tag)))
                .ForMember(sd => sd.Score, opts => opts.MapFrom(s => s.Votes.Count));

            CreateMap<Story, StoryCard>()
                .ForMember(sd => sd.Tags, opts => opts.MapFrom(s => s.StoryTags.Select(st => st.Tag)));

            // Tag mappings
            CreateMap<Tag, TagDto>();
        }
    }
}