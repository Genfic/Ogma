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
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes.Queries
{
    public static class GetIssuedInviteCodes
    {
        public sealed record Query : IRequest<ActionResult<List<InviteCodeDto>>>;

        public class Handler : IRequestHandler<Query, ActionResult<List<InviteCodeDto>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly long? _uid;

            public Handler(ApplicationDbContext context, IMapper mapper, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _uid = userService.User?.GetNumericId();
            }

            public async Task<ActionResult<List<InviteCodeDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var codes = await _context.InviteCodes
                    .Where(ic => ic.IssuedById == _uid)
                    .ProjectTo<InviteCodeDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new OkObjectResult(codes);
            }
        }
    }
}