using System.Collections.Generic;
using Ogma3.Data.Roles;

namespace Ogma3.Data.Users
{
    public class UserSimpleDto
    {
        public string UserName { get; init; }
        public string Avatar { get; init; }
        public string Title { get; init; }
        public IEnumerable<RoleDto> Roles { get; init; }
    }
}