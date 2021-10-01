using AutoMapper;

namespace Ogma3.Data.Folders;

public class FolderMinimalDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
        
    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<Folder, FolderMinimalDto>();
    }
}