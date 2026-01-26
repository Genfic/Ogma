using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<RemoveBookFromShelf.Result>>;

[Handler]
[MapDelete("api/ShelfStories")]
[Authorize]
public static partial class RemoveBookFromShelf
{
	[Validate]
	public sealed partial record Command(long ShelfId, long StoryId) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		[FromBody]Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.UserId is not {} uid) return TypedResults.Unauthorized();

		var (shelfId, storyId) = request;

		var affectedRows = await context.ShelfStories
			.Where(ss => ss.ShelfId == shelfId)
			.Where(ss => ss.StoryId == storyId)
			.Where(ss => ss.Shelf.OwnerId == uid)
			.ExecuteDeleteAsync(cancellationToken);

		await context.SaveChangesAsync(cancellationToken);
		return affectedRows > 0
			? TypedResults.Ok(new Result(shelfId, storyId))
			: TypedResults.NotFound();
	}

	public sealed record Result(long ShelfId, long StoryId);
}