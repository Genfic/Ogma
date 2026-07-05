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

using ReturnType = Results<Ok<ShelfDto>, NotFound, UnauthorizedHttpResult>;

[Handler]
[MapGroup<ApiGroup>]
[MapGet("shelves/{shelfId:long}")]
[Authorize]
public sealed partial class GetShelf(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.WithName(nameof(GetShelf))
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Query request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var shelf = await context.Shelves
			.Where(s => s.Id == request.ShelfId)
			.Where(s => s.IsPublic || s.OwnerId == uid)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return shelf is null
			? TypedResults.NotFound()
			: TypedResults.Ok(shelf);
	}

	[Validate]
	public sealed partial record Query(long ShelfId) : IValidationTarget<Query>;
}