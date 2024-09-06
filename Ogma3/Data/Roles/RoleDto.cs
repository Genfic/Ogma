using AutoMapper;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Data.Roles;

public class RoleDto
{
	public required long Id { get; init; }
	public required string Name { get; init; }
	public required string? Color { get; init; }
	public required bool IsStaff { get; init; }
	public required byte Order { get; init; }

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<OgmaRole, RoleDto>();
	}
}

[Mapper]
public static partial class RoleMapper
{
	public static partial IQueryable<RoleDto> ProjectToDto(this IQueryable<OgmaRole> queryable);
	public static partial RoleDto ToDto(this OgmaRole queryable);
}