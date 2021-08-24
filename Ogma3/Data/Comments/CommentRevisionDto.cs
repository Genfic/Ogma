using System;
using AutoMapper;
using Markdig;
using Ogma3.Infrastructure.Constants;

namespace Ogma3.Data.Comments
{
    public class CommentRevisionDto
    {
        public DateTime EditTime { get; set; }
        public string Body { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<CommentRevision, CommentRevisionDto>()
                .ForMember(crd => crd.Body, opts
                    => opts.MapFrom(cr => Markdown.ToHtml(cr.Body, MarkdownPipelines.Comment, null))
                );
        }
    }
}