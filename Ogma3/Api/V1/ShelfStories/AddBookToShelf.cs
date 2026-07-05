using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories;

using ReturnType = Results<UnauthorizedHttpResult, NotFound, Ok<AddBookToShelf.Result>>;

[Handler]
[MapGroup<ApiGroup>]
[MapPost("ShelfStories")]
[Authorize]
public sealed partial class AddBookToShelf(ApplicationDbContext context, IUserService userService)
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
		return TypedResults.Ok(new Result(shelfId, storyId));
	}

	[Validate]
	public sealed partial record Command(long ShelfId, long StoryId) : IValidationTarget<Command>;

	public sealed record Result(long ShelfId, long StoryId);
}