using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.InviteCodes;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.CodeGenerator;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.InviteCodes;

using ReturnType = Results<UnauthorizedHttpResult, BadRequest<string>, Ok<InviteCodeDto>>;

[Handler]
[MapPost("api/InviteCodes")]
[Authorize]
public static partial class IssueInviteCode
{
	public sealed record Command;

	private static async ValueTask<ReturnType> HandleAsync(
		Command _,
		ApplicationDbContext context,
		OgmaConfig config,
		ICodeGenerator codeGenerator,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var issuedCount = await context.InviteCodes
			.Where(ic => ic.IssuedById == uid)
			.CountAsync(cancellationToken);

		if (issuedCount >= config.MaxInvitesPerUser)
		{
			return TypedResults.BadRequest($"You cannot generate more than {config.MaxInvitesPerUser} codes");
		}

		var code = new InviteCode
		{
			Code = codeGenerator.GetInviteCode(true),
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