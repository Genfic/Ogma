using System;
using AutoMapper;

namespace Ogma3.Data.InviteCodes;

public class InviteCodeDto
{
	public long Id { get; init; }
	public string Code { get; init; } = null!;
	public string NormalizedCode { get; init; } = null!;
	public string? UsedByUserName { get; init; }
	public string IssuedByUserName { get; init; } = null!;
	public DateTime IssueDate { get; init; }
	public DateTime? UsedDate { get; init; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<InviteCode, InviteCodeDto>();
	}
}