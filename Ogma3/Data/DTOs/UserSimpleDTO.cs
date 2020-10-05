using System.Collections.Generic;

namespace Ogma3.Data.DTOs
{
    public class UserSimpleDto
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Title { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; }
    }
}