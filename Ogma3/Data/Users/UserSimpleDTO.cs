using System.Collections.Generic;
using Ogma3.Data.Roles;

namespace Ogma3.Data.Users
{
    public class UserSimpleDto
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Title { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; }
    }
}