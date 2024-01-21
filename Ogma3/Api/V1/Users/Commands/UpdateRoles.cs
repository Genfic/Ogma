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
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;
using Serilog;

namespace Ogma3.Api.V1.Users.Commands;

public static class UpdateRoles
{
	public sealed record Command(long UserId, IEnumerable<long> Roles) : IRequest<IActionResult>;

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command, IActionResult>
	{
		public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var (userId, roles) = request;

			// Check if user is logged in
			if (userService.User?.GetNumericId() is not {} uid) return Unauthorized();
			if (userService.User?.GetUsername() is not {} username) return Unauthorized();

			var user = await context.Users
				.Where(u => u.Id == userId)
				.Include(u => u.Roles)
				.FirstOrDefaultAsync(cancellationToken);

			if (user is null) return NotFound();

			var newRoles = await context.OgmaRoles
				.Where(ur => roles.Contains(ur.Id))
				.ToListAsync(cancellationToken);

			// Handle role removal
			var removedRoles = user.Roles.Except(newRoles, new OgmaRoleComparer()).ToList();
			foreach (var role in removedRoles)
			{
				user.Roles.Remove(role);
			}

			context.ModeratorActions.AddRange(removedRoles.Select(r => new ModeratorAction
			{
				StaffMemberId = uid,
				Description = ModeratorActionTemplates.UserRoleRemoved(user, username, r.Name)
			}));

			// Handle role adding
			var addedRoles = newRoles.Except(user.Roles, new OgmaRoleComparer()).ToList();
			foreach (var role in addedRoles)
			{
				user.Roles.Add(role);
			}

			context.ModeratorActions.AddRange(addedRoles.Select(r => new ModeratorAction
			{
				StaffMemberId = uid,
				Description = ModeratorActionTemplates.UserRoleAdded(user, username, r.Name)
			}));

			try
			{
				await context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception e)
			{
				Log.Error(e, "Exception occurred when staff member {Staff} tried adding roles {Role} to user {User}", uid, roles, userId);
				return BadRequest();
			}

			return Ok();
		}
	}
}