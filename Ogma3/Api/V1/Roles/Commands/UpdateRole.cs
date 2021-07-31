using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1.Roles.Commands
{
    public static class UpdateRole
    {
        public sealed record Query(long Id, string Name, bool IsStaff, string Color, byte? Order) : IRequest<ActionResult<RoleDto>>;

        public class Handler : IRequestHandler<Query, ActionResult<RoleDto>>
        {
            private readonly RoleManager<OgmaRole> _roleManager;
            public Handler(RoleManager<OgmaRole> roleManager) => _roleManager = roleManager;

            public async Task<ActionResult<RoleDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var (id, name, isStaff, color, order) = request;

                var role = await _roleManager.FindByIdAsync(id.ToString());
                
                role.Name = name;
                role.IsStaff = isStaff;
                role.Color = color;
                role.Order = order;
                
                await _roleManager.UpdateAsync(role);
   
                return new CreatedAtActionResult(
                    nameof(RolesController.GetRole),
                    nameof(RolesController)[..^10],
                    new { role.Id },
                    role
                );
            }
        }
    }
}