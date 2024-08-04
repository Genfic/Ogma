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

using ReturnType = Results<UnauthorizedHttpResult, Ok<ShelfResult[]>>;

[Handler]
[MapGet("api/ShelfStories/{storyId:long}")]
[Authorize]
public static partial class GetPaginatedUserShelves
{
	[UsedImplicitly]
	public sealed record Query(long StoryId, int Page);
	
	private static async ValueTask<ReturnType> HandleAsync(
		Query request,
		ApplicationDbContext context,
		IUserService userService,
		OgmaConfig config,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var (storyId, page) = request;
		var shelves = await context.Shelves
			.Where(s => s.OwnerId == uid)
			.Where(s => !s.IsQuickAdd)
			.OrderBy(s => s.Id)
			.Paginate(page, config.ShelvesPerPage)
			.Select(s => new ShelfResult(
				s.Id,
				s.Name,
				s.Color,
				s.Icon != null ? s.Icon.Name : null,
				s.Stories.Any(x => x.Id == storyId)
			))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(shelves);
	}
}

public sealed record ShelfResult(long Id, string Name, string? Color, string? IconName, bool DoesContainBook);