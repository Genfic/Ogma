using System.Linq;
using AutoMapper;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<OgmaUser, ProfileBar>()
                .ForMember(pb => pb.Roles, opts => opts.MapFrom(u => u.UserRoles.Select(ur => ur.Role)))
                .ForMember(pb => pb.StoriesCount, opts => opts.MapFrom(u => u.Stories.Count(s => s.IsPublished)))
                .ForMember(pb => pb.BlogpostsCount, opts => opts.MapFrom(u => u.Blogposts.Count(b => b.IsPublished)));
            CreateMap<OgmaUser, UserProfileDto>()
                .ForMember(pb => pb.Roles, opts => opts.MapFrom(u => u.UserRoles.Select(ur => ur.Role)))
                .ForMember(pb => pb.StoriesCount, opts => opts.MapFrom(u => u.Stories.Count(s => s.IsPublished)))
                .ForMember(pb => pb.BlogpostsCount, opts => opts.MapFrom(u => u.Blogposts.Count(b => b.IsPublished)));
            
            // Role mappings
            CreateMap<OgmaRole, RoleDTO>();
            
            // Chapter mappings
            CreateMap<Chapter, ChapterDetails>();
            CreateMap<Chapter, ChapterBasicDto>();

            // Story mappings
            CreateMap<Story, StoryDetails>()
                .ForMember(sd => sd.CommentsCount, opts => opts.MapFrom(s => s.Chapters.Sum(c => c.CommentsThread.CommentsCount)))
                .ForMember(sd => sd.Tags, opts => opts.MapFrom(s => s.StoryTags.Select(st => st.Tag)))
                .ForMember(sd => sd.Score, opts => opts.MapFrom(s => s.Votes.Count));

            CreateMap<Story, StoryCard>()
                .ForMember(sd => sd.Tags, opts => opts.MapFrom(s => s.StoryTags.Select(st => st.Tag)));

            // Bookshelf mappings
            CreateMap<Shelf, BookshelfDetails>()
                .ForMember(s => s.Stories, opts => opts.MapFrom(s => s.ShelfStories.Select(ss => ss.Story)));
            
            // Tag mappings
            CreateMap<Tag, TagDto>();
            
            // Blogpost mappings
            CreateMap<Blogpost, BlogpostCard>();
            CreateMap<Blogpost, BlogpostDetails>()
                .ForMember(bd => bd.CommentsCount, opts => opts.MapFrom(b => b.CommentsThread.CommentsCount));
        }
    }
}