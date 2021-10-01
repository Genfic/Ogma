using System;
using System.Linq.Expressions;

namespace Ogma3.Data.Roles;

public static class RoleMappings
{
    public static readonly Expression<Func<OgmaRole, RoleDto>> ToRoleDto = r => new RoleDto
    {
        Id = r.Id,
        Name = r.Name,
        Order = r.Order ?? 0,
        IsStaff = r.IsStaff,
        Color = r.Color
    };
}