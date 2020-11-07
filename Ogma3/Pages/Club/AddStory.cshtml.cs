using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club
{
    public class AddStoryModel : PageModel
    {
        private readonly ClubRepository _clubRepo;
        private readonly StoriesRepository _storiesRepo;
        private readonly ApplicationDbContext _context;

        public AddStoryModel(ClubRepository clubRepo, StoriesRepository storiesRepo, ApplicationDbContext context)
        {
            _clubRepo = clubRepo;
            _storiesRepo = storiesRepo;
            _context = context;
        }

        public ClubBar ClubBar { get; set; }
        public StoryCard StoryCard { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, long storyId)
        {
            ClubBar = await _clubRepo.GetClubBar(id);
            if (ClubBar == null) return NotFound();
            
            StoryCard = await _storiesRepo.GetCard(storyId);
            if (StoryCard == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(PostData data)
        {
            var folder = await _context.Folders
                .Where(f => f.Id == data.ParentId)
                .Where(f => f.ClubId == data.ClubId)
                .Include(f => f.Stories)
                .FirstOrDefaultAsync();
            if (folder == null) return NotFound();

            var story = await _context.Stories
                .Where(s => s.Id == data.StoryId)
                .Where(s => s.IsPublished)
                .FirstOrDefaultAsync();
            if (story == null) return NotFound();
            
            folder.Stories.Add(story);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Folders/Folder", new {clubId = folder.ClubId, id = folder.Id, slug = folder.Slug});
        }
        
        public class PostData
        {
            public long StoryId { get; set; }
            public long ClubId { get; set; }
            public long ParentId { get; set; } // folder
        }
    }
}