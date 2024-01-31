using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.UserActivity.Commands;

public static class UpdateLastActive
{
	public sealed record Command : IRequest<ActionResult<int>>;

	public class Handler(ApplicationDbContext context, IUserService userService) : BaseHandler, IRequestHandler<Command, ActionResult<int>>
	{
		public async Task<ActionResult<int>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not { } uid) return NotFound();
			
			var rows = await context.Users
				.Where(u => u.Id == uid)
				.ExecuteUpdateAsync(
				setters => setters.SetProperty(u => u.LastActive, DateTime.Now.ToUniversalTime()),
				cancellationToken
			);
			return Ok(rows);
		}
	}
}