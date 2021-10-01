using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Folders;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Services.UserService;

namespace Ogma3.Api.V1.Folders.Commands;

public static class AddStoryToFolder
{
    public sealed record Command(long FolderId, long StoryId) : IRequest<ActionResult<FolderStory>>;

    public class Handler : IRequestHandler<Command, ActionResult<FolderStory>>
    {
        private readonly ApplicationDbContext _context;
        private readonly long? _uid;

        public Handler(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _uid = userService?.User?.GetNumericId();
        }
            
        public async Task<ActionResult<FolderStory>> Handle(Command request, CancellationToken cancellationToken)
        {
            var (folderId, storyId) = request;
            
            var folderExists = await _context.Folders
                .Where(f => f.Id == folderId)
                .Where(f => f.Club.ClubMembers.FirstOrDefault(c => c.MemberId == _uid).Role <= f.AccessLevel)
                .AnyAsync(cancellationToken);
            if (!folderExists) return new NotFoundObjectResult("Folder not found or insufficient permissions");

            var storyExists = await _context.Stories
                .AnyAsync(s => s.Id == storyId, cancellationToken);
            if (!storyExists) return new NotFoundObjectResult("Story not found");
            
            var exists = await _context.FolderStories
                .AnyAsync(fs => fs.FolderId == folderId && fs.StoryId == storyId, cancellationToken);
            if (exists) return new ConflictObjectResult("Already exists");

            var entity = _context.FolderStories.Add(new FolderStory
            {
                FolderId = folderId,
                StoryId = storyId,
            });
            
            await _context.SaveChangesAsync(cancellationToken);
            return new OkObjectResult(entity.Entity);
        }
    }
}