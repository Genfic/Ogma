using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Bases;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Constants;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ContentBlockController))]
    [ApiController]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class ContentBlockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ContentBlockController(ApplicationDbContext context) => _context = context;

        [HttpPost("story")]
        public async Task<ActionResult> BlockStory(PostData data) => await BlockContent<Story>(data);
        
        [HttpPost("chapter")]
        public async Task<ActionResult> BlockChapter(PostData data) => await BlockContent<Chapter>(data);
        
        [HttpPost("blogpost")]
        public async Task<ActionResult> BlockBlogpost(PostData data) => await BlockContent<Blogpost>(data);
        
        private async Task<ActionResult> BlockContent<T>(PostData data) where T : BaseModel, IBlockableContent
        {
            var (itemId, reason) = data;

            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            
            var item = await _context.Set<T>()
                .Where(i => i.Id == itemId)
                .FirstOrDefaultAsync();
            if (item is null) return NotFound();

            item.ContentBlock = new ContentBlock
            {
                Reason = reason,
                IssuerId = (long) uid
            };
            await _context.SaveChangesAsync();

            return Ok();
        }
        public sealed record PostData(long ObjectId, string Reason);
        
        
        // Don't delete or this whole controller will break
        [HttpGet] public string Ping() => "Pong";
    }
}