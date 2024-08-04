using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<RemoveBookFromShelfResult>>;

[Handler]
[MapDelete("api/ShelfStories")]
[Authorize]
public static partial class RemoveBookFromShelf
{
	[UsedImplicitly]
	public sealed record Command(long ShelfId, long StoryId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		[FromBody]Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var (shelfId, storyId) = request;

		var affectedRows = await context.ShelfStories
			.Where(ss => ss.ShelfId == shelfId)
			.Where(ss => ss.StoryId == storyId)
			.Where(ss => ss.Shelf.OwnerId == uid)
			.ExecuteDeleteAsync(cancellationToken);

		await context.SaveChangesAsync(cancellationToken);
		return affectedRows > 0 
			? TypedResults.Ok(new RemoveBookFromShelfResult(shelfId, storyId)) 
			: TypedResults.NotFound();
	}
}

public sealed record RemoveBookFromShelfResult(long ShelfId, long StoryId);