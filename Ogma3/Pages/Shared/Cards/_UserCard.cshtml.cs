using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Pages.Shared.Cards;

public class UserCard
{
	public string UserName { get; set; }
	public string Avatar { get; set; }
	public string Title { get; set; }
	public IEnumerable<RoleDto> Roles { get; set; }

	public class MappingProfile : Profile
	{
		public MappingProfile() =>
			CreateMap<OgmaUser, UserCard>()
				.ForMember(pb => pb.Roles,
					opts => opts.MapFrom(u => u.Roles.OrderByDescending(r => r.Order)));
	}
}