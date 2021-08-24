using System;
using AutoMapper;
using Ogma3.Data.Stories;

namespace Ogma3.Pages.Shared.Minimals
{
    public class StoryMinimal
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string AuthorUserName { get; set; }
        public string Slug { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool IsPublished { get; set; }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Story, StoryMinimal>();
        }
    }
}