using System;
using AutoMapper;
using Ogma3.Data.Blogposts;

namespace Ogma3.Pages.Shared.Cards;

public class BlogpostCard
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string AuthorUserName { get; set; }
    public string Body { get; set; }
    public int WordCount { get; set; }
    public string[] Hashtags { get; set; }
        
    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<Blogpost, BlogpostCard>();
    }
}