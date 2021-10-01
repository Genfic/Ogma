using System;
using AutoMapper;
using Ogma3.Data.Blogposts;

namespace Ogma3.Pages.Shared.Minimals;

public class BlogpostMinimal
{
    public long Id { get; set; }
    public string AuthorUserName { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public DateTime PublishDate { get; set; }
        
    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<Blogpost, BlogpostMinimal>();
    }
}