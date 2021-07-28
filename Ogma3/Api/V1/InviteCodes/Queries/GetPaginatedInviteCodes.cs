using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;

namespace Ogma3.Api.V1.InviteCodes.Queries
{
    public static class GetPaginatedInviteCodes
    {
        public sealed record Query(int Page, int PerPage) : IRequest<ActionResult<List<InviteCodeDto>>>;

        public class Handler : IRequestHandler<Query, ActionResult<List<InviteCodeDto>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public Handler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ActionResult<List<InviteCodeDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var (page, perPage) = request;
                
                var codes = await _context.InviteCodes
                    .OrderByDescending(ic => ic.UsedDate)
                    .ThenByDescending(ic => ic.IssueDate)
                    .Paginate(page, perPage)
                    .ProjectTo<InviteCodeDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new OkObjectResult(codes);
            }
        }
    }
}