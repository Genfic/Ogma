using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
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
            Chapter = await _chaptersRepo.GetChapterDetails(id);

            if (Chapter == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
