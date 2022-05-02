using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.MediatR.Bases;
using Ogma3.Infrastructure.Middleware;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Infractions.Commands;

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

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<Response>>
	{
		private readonly ApplicationDbContext _context;
		private readonly long? _uid;
		private readonly IMemoryCache _cache;

		public Handler(ApplicationDbContext context, IUserService userService, IMemoryCache cache)
		{
			_context = context;
			_cache = cache;
			_uid = userService.User?.GetNumericId();
		}

		public async Task<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

			var (userId, reason, dateTime, type) = request;
			var infraction = new Infraction
			{
				IssuedById = (long)_uid,
				UserId = userId,
				Reason = reason,
				ActiveUntil = dateTime,
				Type = type
			};
			_context.Infractions.Add(infraction);
			await _context.SaveChangesAsync(cancellationToken);

			if (infraction.Type == InfractionType.Ban)
			{
				_cache.Set(UserBanMiddleware.CacheKey(infraction.UserId), infraction.ActiveUntil);
			}

			return Ok(new Response(infraction.Id, (long)_uid, userId));
		}
	}

	public sealed record Response(long Id, long IssuedBy, long IssuedAgainst);
}