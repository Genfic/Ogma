using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Details;

namespace Ogma3.Pages
{
    public class ChapterModel : PageModel
    {
        private readonly ChaptersRepository _chaptersRepo;

        public ChapterModel(ChaptersRepository chaptersRepo)
        {
            _chaptersRepo = chaptersRepo;
        }
        
        public ChapterDetails Chapter { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Chapter = await _chaptersRepo.GetChapterDetails(id, User.GetNumericId());

            if (Chapter == null)
            {
                return NotFound();
            }
            
            var siblings = await _chaptersRepo.GetMicroSiblings(Chapter.StoryId, Chapter.Order);
            
            Chapter.Previous = siblings.FirstOrDefault(c => c.Order < Chapter.Order);
            Chapter.Next = siblings.FirstOrDefault(c => c.Order > Chapter.Order);
            
            return Page();
        }
    }
}
