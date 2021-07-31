using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1.Roles.Commands
{
    public static class CreateRole
    {
        public sealed record Query(string Name, bool IsStaff, string Color, byte? Order) : IRequest<ActionResult<RoleDto>>;

        public class Handler : IRequestHandler<Query, ActionResult<RoleDto>>
        {
            private readonly RoleManager<OgmaRole> _roleManager;
            public Handler(RoleManager<OgmaRole> roleManager) => _roleManager = roleManager;

            public async Task<ActionResult<RoleDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var (name, isStaff, color, order) = request;
                if (await _roleManager.RoleExistsAsync(name)) return new ConflictObjectResult($"Role {name} already exists");
                
                var role = new OgmaRole
                {
                    Name = name,
                    IsStaff = isStaff,
                    Color = color,
                    Order = order
                };
                
                await _roleManager.CreateAsync(role);
   
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