using System;
using AutoMapper;
using Ogma3.Data.Chapters;

namespace Ogma3.Pages.Shared.Minimals
{
    public class ChapterMinimal
    {
        public long Id { get; set; }
        public string StoryTitle { get; set; }
        public string StoryAuthorUserName { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Chapter, ChapterMinimal>();
        }
    }
}