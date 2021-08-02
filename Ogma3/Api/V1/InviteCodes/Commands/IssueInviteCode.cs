using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes.Commands
{
    public static class IssueInviteCode
    {
        public sealed record Command : IRequest<ActionResult<InviteCodeDto>>;

        public class Handler : IRequestHandler<Command, ActionResult<InviteCodeDto>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly OgmaConfig _config;
            private readonly ICodeGenerator _codeGenerator;
            private readonly long? _uid;

            public Handler(ApplicationDbContext context, IMapper mapper, OgmaConfig config, ICodeGenerator codeGenerator, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _config = config;
                _codeGenerator = codeGenerator;
                _uid = userService.User?.GetNumericId();
            }
            
            public async Task<ActionResult<InviteCodeDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_uid is null) return new UnauthorizedResult();
                
                var issuedCount = await _context.InviteCodes
                    .Where(ic => ic.IssuedById == _uid)
                    .CountAsync(cancellationToken);
            
                if (issuedCount >= _config.MaxInvitesPerUser) 
                    return new UnauthorizedObjectResult($"You cannot generate more than {_config.MaxInvitesPerUser} codes");
            
                var code = new InviteCode
                {
                    Code = _codeGenerator.GetInviteCode(),
                    IssuedById = (long)_uid
                };
                _context.InviteCodes.Add(code);

                await _context.SaveChangesAsync(cancellationToken);
            
                return _mapper.Map<InviteCode, InviteCodeDto>(code);
            }
        }
    }
}