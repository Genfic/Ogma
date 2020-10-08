using System;
using System.Linq;
using AutoMapper;
using Markdig;
using Ogma3.Data.DTOs;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
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
                    usd => usd.Roles,
                    opts 
                        => opts.MapFrom(u => u.Roles.OrderByDescending(r => r.Order))
                );
            CreateMap<OgmaUser, UserCard>()
                .ForMember(
                    pb => pb.Roles,
                    opts 
                        => opts.MapFrom(u => u.Roles.OrderByDescending(r => r.Order))
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
            
            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(
                    cd => cd.Body,
                    opts
                        => opts.MapFrom(c => Markdown.ToHtml(c.Body, null))
                )
                .ForMember(
                    cd => cd.Owned,
                    opts
                        => opts.MapFrom(c => c.AuthorId == currentUser)
                );
            
            // Comment revision mappings
            CreateMap<CommentRevision, CommentRevisionDto>();
            
            // Invite code mappings
            CreateMap<InviteCode, InviteCodeApiDto>();
            
            // Document mappings
            CreateMap<Document, DocumentDto>();
            CreateMap<Document, DocumentVersionDto>();
        }
    }
}