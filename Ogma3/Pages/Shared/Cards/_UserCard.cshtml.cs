using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Pages.Shared.Cards;

public class UserCard
{
	public required string UserName { get; init; }
	public required string Avatar { get; init; }
	public required string? Title { get; init; }
	public required IEnumerable<RoleDto> Roles { get; init; } = [];

	// TODO: Get rid of Automapper
	public class MappingProfile : Profile
	{
		public MappingProfile() =>
			CreateMap<OgmaUser, UserCard>()
				.ForMember(pb => pb.Roles,
					opts => opts.MapFrom(u => u.Roles.OrderByDescending(r => r.Order)));
	}
}