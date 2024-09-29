using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.Clubs;

[Handler]
[MapGet("api/clubs/story/{storyId:long}")]
public static partial class GetClubsWithStory
{
	[UsedImplicitly]
	public sealed record Query(long StoryId);

	private static async ValueTask<Ok<Result[]>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var clubs = await context.Clubs
			.Select(c => new
			{
				Club = c,
				Folders = c.Folders.Where(f => f.Stories.Any(s => s.Id == request.StoryId)),
			})
			.Where(x => x.Folders.Any())
			.Select(c => new Result(
				c.Club.Id,
				c.Club.Name,
				c.Club.Icon,
				c.Folders.Select(f => f.Name)
			))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(clubs);
	}

	[UsedImplicitly]
	public sealed record Result(long Id, string Name, string Icon, IEnumerable<string> Folders);
}