using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Api.V1.CommentThreads.Queries;
using Ogma3.Data;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;
using Serilog;

namespace Ogma3.Api.V1.CommentThreads.Commands;

public static class LockThread
{
	public sealed record Command(long Id) : IRequest<ActionResult<bool>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<bool>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMediator _mediator;
		private readonly ClaimsPrincipal? _user;
		
		public Handler(ApplicationDbContext context, IMediator mediator, IUserService userService)
		{
			_context = context;
			_mediator = mediator;
			_user = userService.User;
		}

		public async ValueTask<ActionResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_user is null) return Unauthorized();
			if (_user.GetUsername() is not { } uname) return Unauthorized();
			if (_user.GetNumericId() is not { } uid) return Unauthorized();
			
			var permission = await _mediator.Send(new GetPermissions.Query(request.Id), cancellationToken);
			if (!permission.IsAllowed) return Unauthorized();

			var thread = await _context.CommentThreads
				.Where(ct => ct.Id == request.Id)
				.FirstOrDefaultAsync(cancellationToken);

			if (thread is null) return NotFound();

			thread.LockDate = thread.LockDate is null ? DateTime.Now : null;

			if (_user.IsInRole(RoleNames.Admin) || _user.IsInRole(RoleNames.Moderator))
			{
				string type;
				if (thread.BlogpostId is not null) type = "blogpost";
				else if (thread.ChapterId is not null) type = "chapter";
				else if (thread.ClubThreadId is not null) type = "club";
				else if (thread.UserId is not null) type = "user profile";
				else type = "unknown";

				var typeId = thread.BlogpostId ?? thread.ChapterId ?? thread.ClubThreadId ?? thread.UserId ?? 0;

				var message = thread.LockDate is null
					? ModeratorActionTemplates.ThreadUnlocked(type, typeId, thread.Id, uname)
					: ModeratorActionTemplates.ThreadLocked(type, typeId, thread.Id, uname);
				
				if (permission is { IsSiteModerator: true, IsClubModerator: false })
				{
					_context.ModeratorActions.Add(new ModeratorAction
					{
						StaffMemberId = uid,
						Description = message
					});
				}
				else if (permission.IsClubModerator && thread.ClubThreadId is not null)
				{
					var clubId = await _context.ClubThreads
						.Where(ct => ct.CommentsThread.Id == request.Id)
						.Select(ct => ct.ClubId)
						.FirstOrDefaultAsync(cancellationToken);
					_context.ClubModeratorActions.Add(new ClubModeratorAction
					{
						ModeratorId = uid,
						ClubId = clubId,
						Description = message
					});
				}
				else
				{
					Log.Error("Comment thread was locked in an unexpected way. Permission {Permission}", permission);
				}
			}

			await _context.SaveChangesAsync(cancellationToken);

			return Ok(thread.LockDate is not null);
		}
	}
}