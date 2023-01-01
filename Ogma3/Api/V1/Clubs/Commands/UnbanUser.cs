using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Clubs.Commands;

public static class UnbanUser
{
	public sealed record Command(long UserId, long ClubId) : IRequest<ActionResult<bool>>;

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
			if (_uid is not { } uid) return Unauthorized();

			var (userId, clubId) = request;

			var issuer = await _context.ClubMembers
				.Where(c => c.ClubId == clubId)
				.Where(cm => cm.MemberId == _uid)
				.Select(cm => new Issuer(cm.Role, cm.Member.UserName))
				.FirstOrDefaultAsync(cancellationToken);

			if (issuer is null) return Unauthorized();

			// Find users
			var user = await _context.ClubMembers
				.Where(c => c.ClubId == clubId)
				.Where(c => c.MemberId == userId)
				.Select(c => new BannedUser(c.Member.UserName, c.Role))
				.FirstOrDefaultAsync(cancellationToken);

			if (user is null) return NotFound();

			// Check privileges
			if (issuer.Role == EClubMemberRoles.User) return Unauthorized("Insufficient privileges");
			if (issuer.Role > user.Role) return Unauthorized("Can't ban someone with a higher role");

			// Everything is fine, time to unban
			var result = await _context.ClubBans
				.Where(cb => cb.ClubId == clubId && cb.UserId == userId)
				.ExecuteDeleteAsync(cancellationToken);

			if (result <= 0) return ServerError("Something went wrong with the unban");

			// Log it
			_context.ClubModeratorActions.Add(new ClubModeratorAction
			{
				ClubId = clubId,
				ModeratorId = uid,
				Description = ModeratorActionTemplates.UserUnban(user.UserName, issuer.UserName)
			});
			await _context.SaveChangesAsync(cancellationToken);

			return Ok(false);
		}

		private record BannedUser(string UserName, EClubMemberRoles Role);

		private record Issuer(EClubMemberRoles Role, string UserName);
	}
}