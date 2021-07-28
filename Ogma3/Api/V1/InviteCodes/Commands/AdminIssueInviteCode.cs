using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes.Commands
{
    public static class AdminIssueInviteCode
    {
        public sealed record Query() : IRequest<ActionResult<InviteCodeDto>>;

        public class Handler : IRequestHandler<Query, ActionResult<InviteCodeDto>>
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
            
            public async Task<ActionResult<InviteCodeDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();
                
                var code = new InviteCode
                {
                    Code = CodeGenerator.InviteCode(),
                    IssuedById = (long)_uid
                };
                _context.InviteCodes.Add(code);

                await _context.SaveChangesAsync(cancellationToken);
            
                return _mapper.Map<InviteCode, InviteCodeDto>(code);
            }
        }
    }
}