using System;
using System.Linq.Expressions;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;

namespace Ogma3.Data.Mappings
{
    public static class RoleMappings
    {
        public static readonly Expression<Func<OgmaRole, RoleDto>> ToRoleDto = r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Order = (int) (r.Order ?? 0),
            IsStaff = r.IsStaff,
            Color = r.Color
        };
    }
}