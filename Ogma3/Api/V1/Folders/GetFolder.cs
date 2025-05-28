using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Folders;

using ReturnType = Results<Ok<GetFolder.Result[]>, UnauthorizedHttpResult>;

[Handler]
[MapGet("api/folders")]
public static partial class GetFolder
{
	[Validate]
	public sealed partial record Query(long ClubId) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var folders = await context.Folders
			.Where(f => f.ClubId == request.ClubId)
			.Select(f => new Result(
				f.Id,
				f.Name,
				f.Slug,
				// We cannot use `.Any(c => c.Role <= f.AccessLevel)` here, since Postgres has no conversion between
				// enums and integers, not even a concept of an enum *being* an integer.
				f.Club.ClubMembers.First(c => c.MemberId == uid).Role <= f.AccessLevel
			))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(folders);
	}

	public sealed record Result(long Id, string Name, string Slug, bool CanAdd);
}