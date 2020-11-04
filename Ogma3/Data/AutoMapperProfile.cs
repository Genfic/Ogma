using System;
using System.Linq;
using AutoMapper;
using Markdig;
using MarkdigExtensions.Mentions;
using MarkdigExtensions.Spoiler;
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
            
            var md = new MarkdownPipelineBuilder()
                .UseMentions(new MentionOptions("/user/", "_blank"))
                .UseAutoLinks()
                .UseAutoIdentifiers()
                .UseSpoilers()
                .Build();

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
                )
                .ForMember(
                    pb => pb.IsBlockedBy,
                    opts
                    => opts.MapFrom(u => u.BlacklistedBy.Any(bu => bu.UserId == currentUser))
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
                )
                .ForMember(
                    pb => pb.IsBlockedBy,
                    opts
                        => opts.MapFrom(u => u.BlacklistedBy.Any(bu => bu.UserId == currentUser))
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
            CreateMap<Chapter, ChapterMinimal>();

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
            CreateMap<Story, StoryMinimal>();

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
            CreateMap<Blogpost, BlogpostMinimal>();
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
                )
                .ForMember(
                    cb => cb.StoriesCount,
                    opts
                    => opts.MapFrom(c => c.Folders.Sum(f => f.StoriesCount))
                );
            CreateMap<Club, ClubCard>();
            
            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(
                    cd => cd.Body,
                    opts
                        => opts.MapFrom(c => Markdown.ToHtml(c.Body, md))
                )
                .ForMember(
                    cd => cd.Owned,
                    opts
                        => opts.MapFrom(c => c.AuthorId == currentUser)
                )
                .ForMember(
                    cd => cd.IsBlocked,
                    opts
                    => opts.MapFrom(c => c.Author.BlacklistedBy.Any(bu => bu.UserId == currentUser))
                );
            
            // Comment revision mappings
            CreateMap<CommentRevision, CommentRevisionDto>()
                .ForMember(
                    crd => crd.Body,
                    opts
                        => opts.MapFrom(cr => Markdown.ToHtml(cr.Body, md))
                );
            
            // Folder mappings
            CreateMap<Folder, FolderCard>();
            CreateMap<Folder, FolderMinimal>();
            CreateMap<Folder, FolderMinimalDto>();
            CreateMap<Folder, FolderMinimalWithParentDto>();
            CreateMap<Folder, FolderDto>();
            
            // Invite code mappings
            CreateMap<InviteCode, InviteCodeApiDto>();
            
            // Document mappings
            CreateMap<Document, DocumentDto>();
            CreateMap<Document, DocumentVersionDto>();
        }
    }
}