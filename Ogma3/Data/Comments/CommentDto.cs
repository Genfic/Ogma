using System;
using System.Linq;
using AutoMapper;
using Markdig;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Comments
{
    public class CommentDto
    {
        public long Id { get; set; }
        public UserSimpleDto? Author { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? LastEdit { get; set; }
        public ushort EditCount { get; set; }
        public bool Owned { get; set; }
        public string Body { get; set; }
        public EDeletedBy? DeletedBy { get; set; }
        public bool IsBlocked { get; set; }
        
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                long? currentUser = null;
                
                CreateMap<Comment, CommentDto>()
                    .ForMember(cd => cd.Owned, opts
                            => opts.MapFrom(c => c.AuthorId == currentUser)
                    )
                    .ForMember(cd => cd.IsBlocked, opts
                            => opts.MapFrom(c => c.Author.BlockedByUsers.Any(bu => bu.Id == currentUser))
                    )
                    .ForMember(cd => cd.Author, opts
                            => opts.MapFrom(c => c.DeletedBy == null ? c.Author : null)
                    )
                    .ForMember(cd => cd.Body, opts
                            => opts.MapFrom(c => c.DeletedBy == null ? Markdown.ToHtml(c.Body, MarkdownPipelines.Comment, null) : null)
                    );
            }
        }
    }
}