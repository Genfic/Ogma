using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Folders;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Folders;

using ReturnType = Results<UnauthorizedHttpResult, NotFound<string>, Conflict<string>, Ok<AddStoryToFolder.Response>>;

[Handler]
[MapPost("api/folders/AddStory")]
public static partial class AddStoryToFolder
{
	[Validate]
	public sealed partial record Command(long FolderId, long StoryId) : IValidationTarget<Command>;

	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		IUserService userService,
		CancellationToken cancellationToken
	)
	{
		if (userService.User?.GetNumericId() is not {} uid) return TypedResults.Unauthorized();

		var folder = await context.Folders
			.Where(f => f.Id == request.FolderId)
			.Where(f => f.Club.ClubMembers.First(c => c.MemberId == uid).Role <= f.AccessLevel)
			.AnyAsync(cancellationToken);

		if (!folder) return TypedResults.NotFound("Folder not found or insufficient permissions");

		var storyExists = await context.Stories
			.Where(s => s.Id == request.StoryId)
			.AnyAsync(cancellationToken);
		if (!storyExists) return TypedResults.NotFound("Story not found");

		var exists = await context.FolderStories
			.Where(fs => fs.FolderId == request.FolderId)
			.Where(fs => fs.StoryId == request.StoryId)
			.AnyAsync(cancellationToken);
		if (exists) return TypedResults.Conflict("Already exists");

		var fs = new FolderStory
		{
			FolderId = request.FolderId,
			StoryId = request.StoryId,
			AddedById = uid,
		};
		context.FolderStories.Add(fs);

		await context.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok(new Response(fs.FolderId, fs.StoryId, fs.Added, fs.AddedById));
	}

	public sealed record Response(long FolderId, long StoryId, DateTimeOffset Added, long AddedById);
}