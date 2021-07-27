using System.Linq;
using AutoMapper;
using Markdig;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Clubs;
using Ogma3.Data.Comments;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Folders;
using Ogma3.Data.InviteCodes;
using Ogma3.Data.Ratings;
using Ogma3.Data.Reports;
using Ogma3.Data.Roles;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            long? currentUser = null;

            var md = MarkdownPipelines.Comment;

            // User mappings
            CreateMap<OgmaUser, UserCard>()
                .ForMember(
                    pb => pb.Roles,
                    opts
                        => opts.MapFrom(u => u.Roles.OrderByDescending(r => r.Order))
                );

            // Role mappings
            CreateMap<OgmaRole, RoleDto>();

            // Chapter mappings
            CreateMap<Chapter, ChapterMinimal>();

            // Story mappings
            CreateMap<Story, StoryCard>()
                .ForMember(sc => sc.Tags, opts
                    => opts.MapFrom(s => s.Tags.OrderByDescending(t => t.Namespace.HasValue).ThenByDescending(t => t.Namespace))
                );

            CreateMap<Story, StoryMinimal>();

            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(
                    cd => cd.Owned,
                    opts
                        => opts.MapFrom(c => c.AuthorId == currentUser)
                )
                .ForMember(
                    cd => cd.IsBlocked,
                    opts
                        => opts.MapFrom(c => c.Author.BlockedByUsers.Any(bu => bu.Id == currentUser))
                )
                .ForMember(
                    cd => cd.Author,
                    opts
                        => opts.MapFrom(c => c.DeletedBy == null ? c.Author : null)
                )
                .ForMember(
                    cd => cd.Body,
                    opts
                        => opts.MapFrom(c => c.DeletedBy == null ? Markdown.ToHtml(c.Body, md, null) : null)
                );
            
            // CommentsThread mappings
            CreateMap<CommentsThread, CommentsThreadDto>();

            // Comment revision mappings
            CreateMap<CommentRevision, CommentRevisionDto>()
                .ForMember(
                    crd => crd.Body,
                    opts
                        => opts.MapFrom(cr => Markdown.ToHtml(cr.Body, md, null))
                );

            // Blogpost mappings
            CreateMap<Blogpost, BlogpostCard>();
            CreateMap<Blogpost, BlogpostMinimal>();

            // Club mappings
            CreateMap<Club, ClubBar>()
                .ForMember(
                    cb => cb.FounderId,
                    opts
                        => opts.MapFrom(c => c.ClubMembers.First(cm => cm.Role == EClubMemberRoles.Founder).MemberId)
                )
                .ForMember(
                    cb => cb.StoriesCount,
                    opts
                        => opts.MapFrom(c => c.Folders.Sum(f => f.StoriesCount))
                )
                .ForMember(
                    cb => cb.Role,
                    opts
                        => opts.MapFrom(c => c.ClubMembers.Any(cm => cm.MemberId == currentUser)
                            ? c.ClubMembers.First(cm => cm.MemberId == currentUser).Role 
                            : (EClubMemberRoles?) null)
                );
            CreateMap<Club, ClubCard>();

            // Rating mappings
            CreateMap<Rating, RatingDto>();

            // Folder mappings
            CreateMap<Folder, FolderCard>();
            CreateMap<Folder, FolderMinimal>();
            CreateMap<Folder, FolderMinimalDto>();
            CreateMap<Folder, FolderMinimalWithParentDto>();

            // Invite code mappings
            CreateMap<InviteCode, InviteCodeApiDto>();

            // Report mappings
            CreateMap<Report, ReportDto>();
        }
    }
}