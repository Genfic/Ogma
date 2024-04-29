using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.ModeratorActions;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Infrastructure.Middleware;
using Ogma3.Services.UserService;

namespace Ogma3.Areas.Admin.Api.V1.Infractions.Commands;

public static class CreateInfraction
{
	public sealed record Command(long UserId, string Reason, DateTime EndDate, InfractionType Type) : IRequest<ActionResult<Response>>;

	public class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator()
		{
			RuleFor(c => c.UserId).NotNull();
			RuleFor(c => c.Reason).NotEmpty();
			RuleFor(c => c.EndDate).NotNull().GreaterThan(DateTime.Now);
			RuleFor(c => c.Type).NotNull();
		}
	}

	public class Handler(ApplicationDbContext context, IUserService userService, IMemoryCache cache) : BaseHandler, IRequestHandler<Command, ActionResult<Response>>
	{

		public async ValueTask<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (userService.User?.GetNumericId() is not { } uid) return Unauthorized();
			if (userService.User?.GetUsername() is not { } modName) return Unauthorized();

			var (userId, reason, dateTime, type) = request;
			var infraction = new Infraction
			{
				IssuedById = uid,
				UserId = userId,
				Reason = reason,
				ActiveUntil = dateTime,
				Type = type,
			};
			context.Infractions.Add(infraction);

			var action = new ModeratorAction
			{
				StaffMemberId = uid,
				Description = ModeratorActionTemplates.Infractions.Create(uid, modName, infraction.Id, reason, type),
			};
			context.ModeratorActions.Add(action);

			await context.SaveChangesAsync(cancellationToken);

			if (infraction.Type == InfractionType.Ban)
			{
				cache.Set(UserBanMiddleware.CacheKey(infraction.UserId), infraction.ActiveUntil);
			}

			return Ok(new Response(infraction.Id, uid, userId));
		}
	}

	public sealed record Response(long Id, long IssuedBy, long IssuedAgainst);
}