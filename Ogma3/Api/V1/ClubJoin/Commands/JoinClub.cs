using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ClubJoin.Commands;

public static class JoinClub
{
	public sealed record Command(long ClubId) : IRequest<ActionResult<bool>>;

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
			if (_uid is not { } uid) return Unauthorized();

			var memberOrBanned = await _context.Clubs
				.Where(c => c.Id == request.ClubId)
				.Select(c => new MemberOrBanned(
					c.ClubMembers.Any(cm => cm.MemberId == uid),
					c.BannedUsers.Any(u => u.Id == uid)
				))
				.FirstOrDefaultAsync(cancellationToken);

			if (memberOrBanned is null) return NotFound();
			
			if (memberOrBanned.IsMember) return Ok(true);

			if (memberOrBanned.IsBanned) return Unauthorized("You're banned from this club");

			_context.ClubMembers.Add(new ClubMember
			{
				ClubId = request.ClubId,
				MemberId = uid,
				Role = EClubMemberRoles.User
			});

			await _context.SaveChangesAsync(cancellationToken);

			return Ok(true);
		}

		private record MemberOrBanned(bool IsMember, bool IsBanned);
	}
}