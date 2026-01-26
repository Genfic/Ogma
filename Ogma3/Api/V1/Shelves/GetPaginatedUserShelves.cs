using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<Ok<ShelfDto[]>, UnauthorizedHttpResult>;

[Handler]
[MapGet("api/shelves/{userName:alpha}")]
[Authorize]
public static partial class GetPaginatedUserShelves
{
	[Validate]
	public sealed partial record Query(string UserName, int Page) : IValidationTarget<Query>;

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		OgmaConfig config,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var shelves = await context.Shelves
			.Where(s => s.Owner.NormalizedUserName == request.UserName)
			.Where(s => s.OwnerId == uid || s.IsPublic)
			.Paginate(request.Page, config.ShelvesPerPage)
			.ProjectToDto()
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(shelves);
	}
}