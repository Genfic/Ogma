using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Roles.Queries;

public static class GetAllRoles
{
	public sealed record Query : IRequest<ActionResult<List<RoleDto>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<RoleDto>>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async ValueTask<ActionResult<List<RoleDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var roles = await _context.Roles
				.OrderByDescending(or => or.Order.HasValue)
				.ThenByDescending(or => or.Order)
				.Select(RoleMappings.ToRoleDto)
				.ToListAsync(cancellationToken);

			return Ok(roles);
		}
	}
}