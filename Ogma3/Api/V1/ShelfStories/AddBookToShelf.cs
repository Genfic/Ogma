using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<ShelfAddResult>>;

[Handler]
[MapPost("api/ShelfStories")]
[Authorize]
public static partial class AddBookToShelf
{
	public sealed record Command(long ShelfId, long StoryId);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var (shelfId, storyId) = request;

		var shelfExists = await context.Shelves
			.Where(s => s.Id == shelfId)
			.Where(s => s.OwnerId == uid)
			.AnyAsync(cancellationToken);
		
		if (!shelfExists) return TypedResults.NotFound();

		var storyExists = await context.Stories
			.Where(s => s.Id == storyId)
			.AnyAsync(cancellationToken);
		
		if (!storyExists) return TypedResults.NotFound();

		context.ShelfStories.Add(new ShelfStory
		{
			StoryId = storyId,
			ShelfId = shelfId,
		});

		await context.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok(new ShelfAddResult(shelfId, storyId));
	}


}

public sealed record ShelfAddResult(long ShelfId, long StoryId);