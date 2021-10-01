using System;
using AutoMapper;
using Ogma3.Data.CommentsThreads;

namespace Ogma3.Pages.Shared;

public class CommentsThreadDto
{
    public long Id { get; init; }
    public DateTime? LockDate { get; init; }
    public string Type { get; set; }

    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<CommentsThread, CommentsThreadDto>();
    }
}