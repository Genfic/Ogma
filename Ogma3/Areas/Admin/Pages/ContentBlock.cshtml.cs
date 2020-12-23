using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.AuthorizationData;
using Ogma3.Data.Models;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Areas.Admin.Pages
{
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Moderator)]
    public class ContentBlock : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ContentBlock(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Type { get; set; }
        [BindProperty]
        public long Id { get; set; }
        public Story? Story { get; set; }
        public Chapter? Chapter { get; set; }
        public Blogpost? Blogpost { get; set; }

        public async Task<ActionResult> OnGet(string? type, long? id)
        {
            if (type is null || id is null)
            {
                return Page();
            }
            
            Type = type;
            Id = (long) id;

            switch (type)
            {
                case "story":
                    Story = await _context.Stories
                        .Where(i => i.Id == id)
                        .Include(i => i.ContentBlock)
                        .FirstOrDefaultAsync();
                    break;
                case "chapter":
                    Chapter = await _context.Chapters
                        .Where(i => i.Id == id)
                        .Include(i => i.ContentBlock)
                        .Include(i => i.Story)
                        .FirstOrDefaultAsync();
                    break;
                case "blogpost":
                    Blogpost = await _context.Blogposts
                        .Where(i => i.Id == id)
                        .Include(i => i.ContentBlock)
                        .FirstOrDefaultAsync();
                    break;
                default:
                    return NotFound();
            }

            return Page();
        }

        public async Task<ActionResult> OnPost([FromForm] PostData data)
        {
            var uid = User.GetNumericId();
            if (uid is null) return Unauthorized();
            var luid = (long) uid;

            var result = Type switch
            {
                "story" => await TryBlockUnblockContent<Story>(Id, data.Reason ?? "", luid),
                "chapter" => await TryBlockUnblockContent<Chapter>(Id, data.Reason ?? "", luid),
                "blogpost" => await TryBlockUnblockContent<Blogpost>(Id, data.Reason ?? "", luid),
                _ => false
            };

            if (!result) ModelState.AddModelError("", $"Could not block/unblock the {Type}");

            return RedirectToPage("./ContentBlock", new { type = Type, id = Id });
        }
        
        private async Task<bool> TryBlockUnblockContent<T>(long itemId, string reason, long uid) where T : BaseModel, IBlockableContent
        {
            var modId = User.GetNumericId();
            if (modId is null) return false;
            var luid = (long) modId;
            
            var item = await _context.Set<T>()
                .Where(i => i.Id == itemId)
                .Include(i => i.ContentBlock)
                .FirstOrDefaultAsync();
            if (item is null) return false;

            var title = item switch
            {
                Story s => s.Title,
                Chapter c => c.Title,
                Blogpost b => b.Title,
                _ => ""
            };

            if (item.ContentBlock is null)
            {
                item.ContentBlock = new Data.Models.ContentBlock
                {
                    Reason = reason,
                    IssuerId = uid,
                    Type = typeof(T).Name
                };
                
                // Log the action
                await _context.ModeratorActions.AddAsync(new ModeratorAction
                {
                    StaffMemberId = luid,
                    Description = ModeratorActionTemplates.ContentBlocked(Type, title, itemId, User.GetUsername())
                });
            }
            else
            {
                _context.ContentBlocks.Remove(item.ContentBlock);
            
                await _context.ModeratorActions.AddAsync(new ModeratorAction
                {
                    StaffMemberId = luid,
                    Description = ModeratorActionTemplates.ContentUnblocked(Type, title, itemId, User.GetUsername())
                });
            }
            
            await _context.SaveChangesAsync();

            return true;
        }

        public sealed record PostData(string Reason);
    }
}