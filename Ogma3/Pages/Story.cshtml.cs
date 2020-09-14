using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages
{
    
    public class StoryModel : PageModel
    {
        private readonly UserRepository _userRepo;
        private readonly StoriesRepository _storiesRepo;

        public StoryModel(UserRepository userRepo, StoriesRepository storiesRepo)
        {
            _userRepo = userRepo;
            _storiesRepo = storiesRepo;
        }

        public StoryDetails Story { get; set; }
        public ProfileBar ProfileBar { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Story = await _storiesRepo.GetStoryDetails(id);

            if (Story == null) return NotFound();
            
            ProfileBar = await _userRepo.GetProfileBar(Story.AuthorId);
            
            return Page();
        }
    }
}
