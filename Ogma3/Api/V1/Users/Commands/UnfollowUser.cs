using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users.Commands;

public static class UnfollowUser
{
	public sealed record Command(string Name) : IRequest<ActionResult<bool>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<bool>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var targetUserId = await _context.Users
				.Where(u => u.NormalizedUserName == request.Name.ToUpperInvariant().Normalize())
				.Select(u => u.Id)
				.FirstOrDefaultAsync(cancellationToken);

			var res = await _context.FollowedUsers
				.Where(bu => bu.FollowingUserId == _uid)
				.Where(bu => bu.FollowedUserId == targetUserId)
				.ExecuteDeleteAsync(cancellationToken);

			return res > 0 ? Ok(false) : NotFound();
		}
	}
}