using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.MediatR.Bases;

namespace Ogma3.Api.V1.InviteCodes.Queries;

public static class GetPaginatedInviteCodes
{
	public sealed record Query(int Page, int PerPage) : IRequest<ActionResult<List<InviteCodeDto>>>;

	public class Handler(ApplicationDbContext context)
		: BaseHandler, IRequestHandler<Query, ActionResult<List<InviteCodeDto>>>
	{
		public async ValueTask<ActionResult<List<InviteCodeDto>>> Handle(Query request, CancellationToken cancellationToken)
		{
			var (page, perPage) = request;
			var codes = await context.InviteCodes
				.OrderByDescending(ic => ic.UsedDate)
				.ThenByDescending(ic => ic.IssueDate)
				.Paginate(page, perPage)
				.Select(ic => new InviteCodeDto
				{
					Id = ic.Id,
					Code = ic.Code,
					NormalizedCode = ic.NormalizedCode,
					IssueDate = ic.IssueDate,
					UsedDate = ic.UsedDate,
					IssuedByUserName = ic.IssuedBy.UserName,
					UsedByUserName = ic.UsedBy == null ? null : ic.UsedBy.UserName
				})
				.ToListAsync(cancellationToken);
			
			return Ok(codes);
		}
	}
}