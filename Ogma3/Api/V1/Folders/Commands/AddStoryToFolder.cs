using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Folders;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Infrastructure.Mediator.Bases;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Folders.Commands;

public static class AddStoryToFolder
{
	public sealed record Command(long FolderId, long StoryId) : IRequest<ActionResult<Response>>;
	
	public sealed record Response(long FolderId, long StoryId, DateTime Added, long AddedById)
	{
		public static Response FromFolderStory(FolderStory fs) => new Response(fs.FolderId, fs.StoryId, fs.Added, fs.AddedById);
	}

	public class Handler(ApplicationDbContext context, IUserService userService)
		: BaseHandler, IRequestHandler<Command, ActionResult<Response>>
	{
		private readonly long? _uid = userService.User?.GetNumericId();
		
		public async ValueTask<ActionResult<Response>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (_uid is not {} uid) return Unauthorized();
			
			var (folderId, storyId) = request;

			var folder = await context.Folders
				.Where(f => f.Id == folderId)
				.Where(f => f.Club.ClubMembers.First(c => c.MemberId == _uid).Role <= f.AccessLevel)
				.FirstOrDefaultAsync(cancellationToken);
			
			if (folder is null) return NotFound("Folder not found or insufficient permissions");

			var storyExists = await context.Stories
				.AnyAsync(s => s.Id == storyId, cancellationToken);
			if (!storyExists) return NotFound("Story not found");

			var exists = await context.FolderStories
				.AnyAsync(fs => fs.FolderId == folderId && fs.StoryId == storyId, cancellationToken);
			if (exists) return Conflict("Already exists");

			var entity = context.FolderStories.Add(new FolderStory
			{
				FolderId = folderId,
				StoryId = storyId,
				AddedById = uid
			});
			folder.StoriesCount++;
			
			await context.SaveChangesAsync(cancellationToken);
			return Ok(Response.FromFolderStory(entity.Entity));
		}
	}
}