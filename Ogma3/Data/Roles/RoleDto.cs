using AutoMapper;

namespace Ogma3.Data.Roles;

public class RoleDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string? Color { get; init; }
	public required bool IsStaff { get; init; }
	public required byte? Order { get; init; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<OgmaRole, RoleDto>();
	}
}