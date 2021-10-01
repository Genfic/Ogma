using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Infractions.Commands;

public static class DeactivateInfraction
{
    public sealed record Command(long InfractionId) : IRequest<ActionResult<Response>>;

    public class Handler : IRequestHandler<Command, ActionResult<Response>>
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;
        public Handler(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _uid = userService.User?.GetNumericId();
        }

        public async Task<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (_uid is null) return new UnauthorizedResult();

            var infraction = await _context.Infractions
                .Where(i => i.Id == request.InfractionId)
                .FirstOrDefaultAsync(cancellationToken);
                
            infraction.RemovedAt = DateTime.Now;
            infraction.RemovedById = (long)_uid;

            await _context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new Response(infraction.Id, (long)_uid, infraction.UserId));
        }
    }

    public sealed record Response(long Id, long IssuedBy, long IssuedAgainst);
}