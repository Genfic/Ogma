using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;

namespace Ogma3.Api.V1.Infractions.Queries
{
    public static class GetInfractionDetails
    {
        public sealed record Query(long InfractionId) : IRequest<ActionResult<Result>>;

        public class Handler : IRequestHandler<Query, ActionResult<Result>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context) => _context = context;

            public async Task<ActionResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var infraction = await _context.Infractions
                    .Where(i => i.Id == request.InfractionId)
                    .Select(i => new Result
                    {
                        Id = i.Id,
                        UserName = i.User.UserName,
                        UserId = i.UserId,
                        IssueDate = i.IssueDate,
                        ActiveUntil = i.ActiveUntil,
                        RemovedAt = i.RemovedAt,
                        Reason = i.Reason,
                        Type = i.Type,
                        IssuedByName = i.IssuedBy.UserName,
                        RemovedByName = i.RemovedBy.UserName
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                return new OkObjectResult(infraction);
            }
        }

        public sealed record Result
        {
            public long Id { get; init; }
            public string UserName { get; init; }
            public long UserId { get; init; }
            public DateTime IssueDate { get; init; }
            public DateTime ActiveUntil { get; init; }
            public DateTime? RemovedAt { get; init; }
            public string Reason { get; init; }
            public InfractionType Type { get; init; }
            public string IssuedByName { get; init; }
            public string? RemovedByName { get; init; }
        }
    }
}