using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blacklists;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Shelves;

namespace Ogma3.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly OgmaUserManager _userManager;
        private readonly ApplicationDbContext _context;

        public ConfirmEmailModel(OgmaUserManager userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }
        
        [BindProperty] 
        [Required]
        public string UserName { get; set; }

        [BindProperty]
        [Required]
        public string Code { get; set; }

        public IActionResult OnGet(string userName, string code)
        {
            UserName = userName;
            Code = code;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            
            var user = await _userManager.FindByNameAsync(UserName);
            if (user is null) return NotFound($"Unable to load user with name '{UserName}'.");
            
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

            if (!result.Succeeded) return Page();
            
            // Setup default blacklists
            var defaultBlockedRatings = await _context.Ratings
                .Where(r => r.BlacklistedByDefault)
                .Select(r => r.Id)
                .ToListAsync();
            var blockedRatings = defaultBlockedRatings.Select(dbr => new BlacklistedRating
            {
                User = user,
                RatingId = dbr
            });
            await _context.BlacklistedRatings.AddRangeAsync(blockedRatings);
                
            // Setup profile comment thread subscription
            var thread = await _context.CommentThreads
                .FirstOrDefaultAsync(ct => ct.UserId == user.Id);
            await _context.CommentsThreadSubscribers.AddAsync(new CommentsThreadSubscriber
            {
                CommentsThread = thread,
                OgmaUser = user
            });
                
            // Setup default bookshelves
            var shelves = new Shelf[]
            {
                new() { Name = "Favourites", Description = "My favourite stories", Color = "#ffff00", IsDefault = true, IsPublic = true, TrackUpdates = true, IsQuickAdd = true, Owner = user, IconId = 12 },
                new() { Name = "Read Later", Description = "What I plan to read", Color = "#5555ff", IsDefault = true, IsPublic = true, TrackUpdates = true, IsQuickAdd = true, Owner = user, IconId = 22 },
            };
            await _context.Shelves.AddRangeAsync(shelves);
            
            await _context.SaveChangesAsync();

            return Page();
        }
    }
}
