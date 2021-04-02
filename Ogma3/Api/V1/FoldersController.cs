using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Folders;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(FoldersController))]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly FoldersRepository _foldersRepo;
        private readonly ApplicationDbContext _context;

        public FoldersController(FoldersRepository foldersRepo, ApplicationDbContext context)
        {
            _foldersRepo = foldersRepo;
            _context = context;
        }

        // GET api/folders/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ICollection<FolderMinimalWithParentDto>>> GetChaptersRead(long id)
        {
            var uid = User.GetNumericId();
            if (uid == null) return Unauthorized();
            return Ok(await _foldersRepo.GetClubFolders(id, (long) uid));
        }
        
        [HttpPost("add-story")]
        [Authorize]
        public async Task<ActionResult> AddStory(PostData data)
        {
            var uid = User.GetNumericId();
            var (folderId, storyId) = data;
            
            var folderExists = await _context.Folders
                .Where(f => f.Id == folderId)
                .Where(f => f.Club.ClubMembers.FirstOrDefault(c => c.MemberId == uid).Role <= f.AccessLevel)
                .AnyAsync();
            if (!folderExists) return NotFound("Folder not found or insufficient permissions");

            var storyExists = await _context.Stories
                .AnyAsync(s => s.Id == storyId);
            if (!storyExists) return NotFound("Story not found");
            
            var exists = await _context.FolderStories
                .AnyAsync(fs => fs.FolderId == folderId && fs.StoryId == storyId);
            if (exists) return Conflict("Already exists");
            
            var folderStory = new FolderStory
            {
                FolderId = folderId,
                StoryId = storyId
            };
            
            _context.Entry(folderStory).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return Ok(folderStory);
        }

        [HttpGet]
        public string Ping() => "Pong";

        public sealed record PostData(long FolderId, long StoryId);
    }
}