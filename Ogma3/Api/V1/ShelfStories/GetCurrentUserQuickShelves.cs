using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.ShelfStories;

using ReturnType = Results<UnauthorizedHttpResult, Ok<QuickShelvesResult[]>>;

[Handler]
[MapGet("api/ShelfStories/{storyId:long}/quick")]
[Authorize]
public static partial class GetCurrentUserQuickShelves
{
	[UsedImplicitly]
	public sealed record Query(long StoryId);

	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var shelves = await context.Shelves
			.Where(s => s.OwnerId == uid)
			.Where(s => s.IsQuickAdd)
			.Select(s => new QuickShelvesResult(
				s.Id,
				s.Name,
				s.Color,
				s.Icon != null ? s.Icon!.Name : null,
				s.Stories.Any(x => x.Id == request.StoryId)
			))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(shelves);
	}
}
public sealed record QuickShelvesResult(long Id, string Name, string? Color, string? IconName, bool DoesContainBook);