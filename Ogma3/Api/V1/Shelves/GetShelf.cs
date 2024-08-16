using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<Ok<ShelfDto>, NotFound, UnauthorizedHttpResult>;

[Handler]
[MapGet("api/shelves/{shelfId:long}")]
[Authorize]
public static partial class GetShelf
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint) => endpoint.WithName(nameof(GetShelf));

	public sealed record Query(long ShelfId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var shelf = await context.Shelves
			.Where(s => s.Id == request.ShelfId)
			.Where(s => s.IsPublic || s.OwnerId == uid)
			.ProjectToDto()
			.FirstOrDefaultAsync(cancellationToken);

		return shelf is null
			? TypedResults.NotFound()
			: TypedResults.Ok(shelf);
	}
}