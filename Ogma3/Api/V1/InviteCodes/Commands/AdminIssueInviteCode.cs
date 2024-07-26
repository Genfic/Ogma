using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes.Commands;

public static class AdminIssueInviteCode
{
	public sealed record Command : IRequest<ActionResult<InviteCodeDto>>;

	public class Handler : BaseHandler, IRequestHandler<Command, ActionResult<InviteCodeDto>>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ICodeGenerator _codeGenerator;
		private readonly long? _uid;

		public Handler(ApplicationDbContext context, IMapper mapper, ICodeGenerator codeGenerator, IUserService userService)
		{
			_context = context;
			_mapper = mapper;
			_codeGenerator = codeGenerator;
			_uid = userService.User?.GetNumericId();
		}

		public async ValueTask<ActionResult<InviteCodeDto>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is null) return Unauthorized();

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