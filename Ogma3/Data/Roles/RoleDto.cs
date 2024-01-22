using AutoMapper;

namespace Ogma3.Data.Roles;

public class RoleDto
{
	public long Id { get; init; }
	public required string Name { get; init; }
	public string? Color { get; init; }
	public bool IsStaff { get; init; }
	public byte? Order { get; init; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<OgmaRole, RoleDto>();
	}
}