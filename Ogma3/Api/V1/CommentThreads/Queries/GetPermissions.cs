using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.CommentThreads.Queries;

public static class GetPermissions
{
	public sealed record Query(long Id) : IRequest<Result>;

	public class Handler : IRequestHandler<Query, Result>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;
		private readonly ClaimsPrincipal? _user;
		public Handler(ApplicationDbContext context, IUserService userService)
		{
			_context = context;
			_user = userService.User;
			_uid = _user?.GetNumericId();
		}

		public async ValueTask<Result> Handle(Query request, CancellationToken cancellationToken)
		{
			if (_user is null) return new Result(false, false);
			if (_uid is null) return new Result(false, false);
			
			if (_user.Identity is { IsAuthenticated: false }) return new Result(false, false);
			
			var isSiteModerator = _user.IsInRole(RoleNames.Admin) || _user.IsInRole(RoleNames.Moderator);

			var roles = new[] { EClubMemberRoles.Founder, EClubMemberRoles.Admin, EClubMemberRoles.Moderator };
			var isClubModerator = await _context.CommentThreads
				.Where(ct => ct.Id == request.Id)
				.Where(ct => ct.ClubThreadId != null)
				.Select(ct => ct.ClubThread != null && ct.ClubThread.Club.ClubMembers
					.Where(cm => cm.MemberId == _uid)
					.Any(cm => roles.Contains(cm.Role)))
				.FirstOrDefaultAsync(cancellationToken);

			return new Result(isSiteModerator, isClubModerator);
		}
	}

	public sealed record Result(bool IsSiteModerator, bool IsClubModerator)
	{
		public bool IsAllowed => IsSiteModerator || IsClubModerator;
	}
}