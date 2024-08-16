using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Shelves;

using ReturnType = Results<UnauthorizedHttpResult, Ok<long>, NotFound>;

[Handler]
[MapDelete("api/shelves/{shelfId:long}")]
[Authorize]
public static partial class DeleteShelf
{
	public sealed record Command(long ShelfId);

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var res = await context.Shelves
			.Where(s => s.Id == request.ShelfId)
			.Where(s => s.OwnerId == uid)
			.ExecuteDeleteAsync(cancellationToken);

		return res > 0 ? TypedResults.Ok(request.ShelfId) : TypedResults.NotFound();
	}
}