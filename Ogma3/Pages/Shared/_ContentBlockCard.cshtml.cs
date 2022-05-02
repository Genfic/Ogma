using System;
using AutoMapper;
using Ogma3.Areas.Admin.Pages;

namespace Ogma3.Pages.Shared;

public class ContentBlockCard
{
	public string Reason { get; init; }
	public string IssuerUserName { get; init; }
	public DateTime DateTime { get; init; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<ContentBlock, ContentBlockCard>();
	}
}