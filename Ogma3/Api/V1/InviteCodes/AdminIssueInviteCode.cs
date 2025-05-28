using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.ServiceRegistrations;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<UnauthorizedHttpResult, Ok<InviteCodeDto>>;

[Handler]
[MapPost("api/InviteCodes/no-limit")]
[Authorize(AuthorizationPolicies.RequireAdminOrModeratorRole)]
public static partial class AdminIssueInviteCode
{
	public sealed record Command;

	private static async ValueTask<ReturnType> HandleAsync(
		Command _,
		ApplicationDbContext context,
		ICodeGenerator codeGenerator,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var code = new InviteCode
		{
			Code = codeGenerator.GetInviteCode(),
			IssuedById = uid,
		};
		context.InviteCodes.Add(code);

		await context.SaveChangesAsync(cancellationToken);

		var newCode = await context.InviteCodes
			.Where(ic => ic.Id == code.Id)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken: cancellationToken);

		return TypedResults.Ok(newCode);
	}
}