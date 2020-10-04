using System.Linq;
using AutoMapper;
using Ogma3.Data.DTOs;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Pages.Shared;

namespace Ogma3.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            long? currentUser = null;

            // User mappings
            CreateMap<OgmaUser, ProfileBar>()
                .ForMember(
                    pb => pb.Roles,
                    opts 
                        => opts.MapFrom(u => u.UserRoles.Select(ur => ur.Role))
                )
                .ForMember(
                    pb => pb.StoriesCount,
                    opts 
                        => opts.MapFrom(u => u.Stories.Count(s => s.IsPublished))
                )
                .ForMember(
                    pb => pb.BlogpostsCount,
                    opts 
                        => opts.MapFrom(u => u.Blogposts.Count(b => b.IsPublished))
                );
            CreateMap<OgmaUser, UserProfileDto>()
                .ForMember(
                    pb => pb.Roles,
                    opts 
                        => opts.MapFrom(u => u.UserRoles.Select(ur => ur.Role))
                )
                .ForMember(
                    pb => pb.StoriesCount,
                    opts 
                        => opts.MapFrom(u => u.Stories.Count(s => s.IsPublished))
                )
                .ForMember(
                    pb => pb.BlogpostsCount,
                    opts 
                        => opts.MapFrom(u => u.Blogposts.Count(b => b.IsPublished))
                );
            CreateMap<OgmaUser, UserSimpleDto>()
                .ForMember(
                    usd => usd.TopRole,
                    opts 
                        => opts.MapFrom(u => u.UserRoles.Select(ur => ur.Role).Where(r => r.Order.HasValue).OrderBy(r => r.Order).FirstOrDefault())
                );
            CreateMap<OgmaUser, UserCard>()
                .ForMember(
                    pb => pb.Roles,
                    opts 
                        => opts.MapFrom(u => u.UserRoles.Select(ur => ur.Role))
                );

            // Role mappings
            CreateMap<OgmaRole, RoleDto>();

            // Chapter mappings
            CreateMap<Chapter, ChapterDetails>();
            CreateMap<Chapter, ChapterBasicDto>();

            // Story mappings
            CreateMap<Story, StoryDetails>()
                .ForMember(
                    sd => sd.CommentsCount,
                    opts 
                        => opts.MapFrom(s => s.Chapters.Sum(c => c.CommentsThread.CommentsCount))
                )
                .ForMember(
                    sd => sd.Tags,
                    opts 
                        => opts.MapFrom(s => s.StoryTags.Select(st => st.Tag))
                )
                .ForMember(
                    sd => sd.Score,
                    opts 
                        => opts.MapFrom(s => s.Votes.Count)
                );

            CreateMap<Story, StoryCard>()
                .ForMember(
                    sd => sd.Tags,
                    opts 
                        => opts.MapFrom(s => s.StoryTags.Select(st => st.Tag))
                );

            // Bookshelf mappings
            CreateMap<Shelf, BookshelfDetails>()
                .ForMember(
                    s => s.Stories,
                    opts 
                        => opts.MapFrom(s => s.ShelfStories.Select(ss => ss.Story).Where(st => st.IsPublished || st.Author.Id == currentUser))
                );

            // Tag mappings
            CreateMap<Tag, TagDto>();

            // Blogpost mappings
            CreateMap<Blogpost, BlogpostCard>();
            CreateMap<Blogpost, BlogpostDetails>()
                .ForMember(
                    bd => bd.CommentsCount,
                    opts 
                        => opts.MapFrom(b => b.CommentsThread.CommentsCount)
                );

            // Club mappings
            CreateMap<Club, ClubBar>()
                .ForMember(
                    cb => cb.FounderId,
                    opts 
                        => opts.MapFrom(c => c.ClubMembers.First(cm => cm.Role == EClubMemberRoles.Founder).MemberId)
                )
                .ForMember(
                    cb => cb.IsMember,
                    opts 
                        => opts.MapFrom(c => c.ClubMembers.Any(cm => cm.MemberId == currentUser))
                );
            CreateMap<Club, ClubCard>();
            
            // Invite code mappings
            CreateMap<InviteCode, InviteCodeApiDto>();
            
            // Document mappings
            CreateMap<Document, DocumentDto>();
            CreateMap<Document, DocumentVersionDto>();
        }
    }
}