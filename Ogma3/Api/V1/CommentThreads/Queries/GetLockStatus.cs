using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.CommentThreads.Queries;

public static class GetLockStatus
{
	public sealed record Query(long Id) : IRequest<ActionResult<bool>>;

	public class Handler : IRequestHandler<Query, ActionResult<bool>>
	{
		private readonly ApplicationDbContext _context;
		public Handler(ApplicationDbContext context) => _context = context;

		public async Task<ActionResult<bool>> Handle(Query request, CancellationToken cancellationToken) 
			=> await _context.CommentThreads
				.Where(ct => ct.Id == request.Id)
				.Select(ct => ct.LockDate != null)
				.FirstOrDefaultAsync(cancellationToken);
	}
}