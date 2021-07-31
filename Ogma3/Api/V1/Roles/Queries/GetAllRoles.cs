using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;

namespace Ogma3.Api.V1.Roles.Queries
{
    public static class GetAllRoles
    {
        public sealed record Query : IRequest<ActionResult<List<RoleDto>>>;

        public class Handler : IRequestHandler<Query, ActionResult<List<RoleDto>>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<ActionResult<List<RoleDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var roles =  await _context.Roles
                    .OrderByDescending(or => or.Order.HasValue)
                        .ThenByDescending(or => or.Order)
                    .Select(RoleMappings.ToRoleDto)
                    .ToListAsync(cancellationToken);

                return new OkObjectResult(roles);
            }
        }
    }
}