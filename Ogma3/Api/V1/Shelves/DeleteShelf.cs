using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<UnauthorizedHttpResult, Ok<long>, NotFound>;

[Handler]
[MapDelete("api/shelves/{shelfId:long}")]
[Authorize]
public sealed partial class DeleteShelf(ApplicationDbContext context, IUserService userService)
{
	internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)
		=> endpoint
			.ProducesValidationProblem();

	private async ValueTask<ReturnType> HandleAsync(
		Command request,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var res = await context.Shelves
			.Where(s => s.Id == request.ShelfId)
			.Where(s => s.OwnerId == uid)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.ShelfId) : TypedResults.NotFound();
	}

	[Validate]
	public sealed partial record Command(long ShelfId) : IValidationTarget<Command>;
}