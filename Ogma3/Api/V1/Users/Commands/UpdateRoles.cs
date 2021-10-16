#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Comparers;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;
using Serilog;

namespace Ogma3.Api.V1.Users.Commands;

public static class UpdateRoles
{
    public sealed record Command(long UserId, IEnumerable<long> Roles) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
        private readonly string? _username;

        public Handler(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _uid = userService.User?.GetNumericId();
            _username = userService.User?.GetUsername();
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var (userId, roles) = request;

            // Check if user is logged in
            if (_uid is null) return new UnauthorizedResult();

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null) return new NotFoundResult();

            var newRoles = await _context.OgmaRoles
                .Where(ur => roles.Contains(ur.Id))
                .ToListAsync(cancellationToken);

            // Handle role removal
            var removedRoles = user.Roles.Except(newRoles, new OgmaRoleComparer()).ToList();
            foreach (var role in removedRoles)
            {
                user.Roles.Remove(role);
            }

            _context.ModeratorActions.AddRange(removedRoles.Select(r => new ModeratorAction
            {
                StaffMemberId = (long)_uid,
                Description = ModeratorActionTemplates.UserRoleRemoved(user, _username, r.Name)
            }));

            // Handle role adding
            var addedRoles = newRoles.Except(user.Roles, new OgmaRoleComparer()).ToList();
            foreach (var role in addedRoles)
            {
                user.Roles.Add(role);
            }

            _context.ModeratorActions.AddRange(addedRoles.Select(r => new ModeratorAction
            {
                StaffMemberId = (long)_uid,
                Description = ModeratorActionTemplates.UserRoleAdded(user, _username, r.Name)
            }));

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception occurred when staff member {Staff} tried adding roles {Role} to user {User}", _uid, roles, userId);
                return new BadRequestResult();
            }

            return new OkResult();
        }
    }
}