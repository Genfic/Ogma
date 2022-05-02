using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Folders.Queries;

public static class GetFolder
{
	public sealed record Query(long Id) : IRequest<ActionResult<List<Result>>>;

	public class Handler : BaseHandler, IRequestHandler<Query, ActionResult<List<Result>>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService?.User?.GetNumericId();
		}

		public async Task<ActionResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var folder = await _context.Folders
				.Where(f => f.ClubId == request.Id)
				.Select(f => new Result(
					f.Id,
					f.Name,
					f.Slug,
					f.ParentFolderId,
					f.Club.ClubMembers.FirstOrDefault(c => c.MemberId == (long)_uid).Role <= f.AccessLevel
				))
				.ToListAsync(cancellationToken);

			return Ok(folder);
		}
	}

	public sealed record Result(long Id, string Name, string Slug, long? ParentFolderId, bool CanAdd);
}