using AutoMapper;

namespace Ogma3.Data.Roles
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Color { get; set; }
        public bool IsStaff { get; set; }
        public int Order { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<OgmaRole, RoleDto>();
        }
    }
}