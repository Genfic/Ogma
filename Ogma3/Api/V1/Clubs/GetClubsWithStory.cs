using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;

namespace Ogma3.Api.V1.Clubs;

[Handler]
[MapGet("api/clubs/story/{storyId:long}")]
public static partial class GetClubsWithStory
{
	public sealed record Query(long StoryId);

	private static async ValueTask<Ok<Result[]>> HandleAsync(
		Query request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var clubs = await context.FolderStories
			.Where(fs => fs.StoryId == request.StoryId)
			.Select(fs => new Result(fs.Folder.ClubId, fs.Folder.Club.Name, fs.Folder.Club.Icon))
			.ToArrayAsync(cancellationToken);

		return TypedResults.Ok(clubs);
	}

	public sealed record Result(long Id, string Name, string Icon);
}