using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.ActionResults;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Clubs.Commands;

public static class BanUser
{
    public sealed record Command(long UserId, long ClubId, string Reason) : IRequest<ActionResult<bool>>;
    
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator() => RuleFor(c => c.Reason).NotEmpty();
    }

    public class Handler : IRequestHandler<Command, ActionResult<bool>>
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
            if (_uid is not {} uid) return new UnauthorizedResult();
            
            var (userId, clubId, reason) = request;
            
            // Find users
            var users = await _context.ClubMembers
                .Where(c => c.ClubId == clubId)
                .Where(c => c.MemberId == _uid || c.MemberId == userId)
                .Select(cm => new
                {
                    cm.Member.Id,
                    cm.Member.UserName,
                    cm.Role
                })
                .ToListAsync(cancellationToken);

            var issuer = users.FirstOrDefault(u => u.Id == _uid);
            if (issuer is null) return new UnauthorizedResult();
            
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user is null) return new NotFoundResult();
            
            // Check privileges
            if (issuer.Role == EClubMemberRoles.User) return new UnauthorizedObjectResult("Insufficient privileges");
            if (issuer.Role > user.Role) return new UnauthorizedObjectResult("Can't ban someone with a higher role");
            
            // Everything is fine, time to ban
            _context.ClubBans.Add(new ClubBan
            {
                ClubId = clubId,
                UserId = userId,
                IssuerId = uid,
                Reason = reason
            });
            var result = await _context.SaveChangesAsync(cancellationToken);

            if (result <= 0) return new ServerErrorResult("Something went wrong with the ban");
            
            // Remove the user from club
            await _context.DeleteRangeAsync<ClubMember>(cm => cm.ClubId == clubId && cm.MemberId == userId, cancellationToken: cancellationToken);

            // Log it
            _context.ClubModeratorActions.Add(new ClubModeratorAction
            {
                ClubId = clubId,
                ModeratorId = uid,
                Description = ModeratorActionTemplates.UserBan(user.UserName, issuer.UserName, reason)
            });
            await _context.SaveChangesAsync(cancellationToken);
            
            return new OkObjectResult(true);
        }
    }
}