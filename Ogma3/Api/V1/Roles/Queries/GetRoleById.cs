using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Roles;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.Roles.Queries;

public static class GetRoleById
{
	public sealed record Query(long RoleId) : IRequest<ActionResult<RoleDto>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<RoleDto>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async Task<ActionResult<RoleDto>> Handle(Query request, CancellationToken cancellationToken)
		{
			var role = await _context.Roles
				.Where(r => r.Id == request.RoleId)
				.FirstOrDefaultAsync(cancellationToken);

			return role is null ? NotFound() : Ok(role);
		}
	}
}