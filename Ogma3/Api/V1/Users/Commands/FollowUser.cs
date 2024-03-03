using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users.Commands;

public static class FollowUser
{
	public sealed record Command(string Name) : IRequest<ActionResult<bool>>;

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command, ActionResult<bool>>
	{		
		public async ValueTask<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();

			var targetUserId = await context.Users
				.Where(u => u.NormalizedUserName == request.Name.ToUpperInvariant().Normalize())
				.Select(u => u.Id)
				.FirstOrDefaultAsync(cancellationToken);

			var exists = await context.FollowedUsers
				.Where(bu => bu.FollowingUserId == uid && bu.FollowedUserId == targetUserId)
				.AnyAsync(cancellationToken);

			if (exists) return Ok(true);

			context.FollowedUsers.Add(new UserFollow
			{
				FollowingUserId = uid,
				FollowedUserId = targetUserId
			});
			await context.SaveChangesAsync(cancellationToken);

			return Ok(true);
		}
	}
}