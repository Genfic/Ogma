using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Users.Commands;

public static class BlockUser
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

		public async Task<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var targetUserId = await _context.Users
				.Where(u => u.NormalizedUserName == request.Name.ToUpperInvariant().Normalize())
				.Select(u => u.Id)
				.FirstOrDefaultAsync(cancellationToken);

			var exists = await _context.BlacklistedUsers
				.Where(bu => bu.BlockingUserId == _uid && bu.BlockedUserId == targetUserId)
				.AnyAsync(cancellationToken);

			if (exists) return Ok(true);

			_context.BlacklistedUsers.Add(new UserBlock
			{
				BlockingUserId = (long)_uid,
				BlockedUserId = targetUserId
			});
			await _context.SaveChangesAsync(cancellationToken);

			return Ok(true);
		}
	}
}