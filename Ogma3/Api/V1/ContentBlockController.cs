using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.AuthorizationData;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Api.V1
{
    [Route("api/[controller]", Name = nameof(ContentBlockController))]
    [ApiController]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class ContentBlockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContentBlockController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpPost("story")]
        public async Task<ActionResult> BlockStory(PostData data) => await BlockContent<Story>(data);
        
        [HttpPost("chapter")]
        public async Task<ActionResult> BlockChapter(PostData data) => await BlockContent<Chapter>(data);
        
        [HttpPost("blogpost")]
        public async Task<ActionResult> BlockBlogpost(PostData data) => await BlockContent<Blogpost>(data);
        
        
        [HttpGet]
        public string Ping() => "Pong";

        private async Task<ActionResult> BlockContent<T>(PostData data) where T : BaseModel, IBlockableContent
        {
            var (itemId, reason) = data;
            
            var item = await _context.Set<T>()
                .Where(i => i.Id == itemId)
                .FirstOrDefaultAsync();
            if (item is null) return NotFound();

            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();

            item.ContentBlock = new ContentBlock
            {
                Reason = reason,
                IssuerId = (long) uid
            };
            await _context.SaveChangesAsync();

            return Ok();
        }

        public sealed record PostData(long ObjectId, string Reason);
    }
}