using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages.User
{
    public class StoriesModel : PageModel
    {
        private readonly UserRepository _userRepo;
        private readonly StoriesRepository _storyRepo;

        public StoriesModel(UserRepository userRepo, StoriesRepository storyRepo)
        {
            _userRepo = userRepo;
            _storyRepo = storyRepo;
        }

        public IList<StoryCard> Stories { get;set; }

        private const int PerPage = 25;
        public ProfileBar ProfileBar { get; set; }
        public bool IsCurrentUser { get; set; }
        public Pagination Pagination { get; set; }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            ProfileBar = await _userRepo.GetProfileBar(name.ToUpper());
            if (ProfileBar == null) return NotFound();

            IsCurrentUser = ProfileBar.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier);

            var storiesCount = await _storyRepo.CountForUser(ProfileBar.Id);

            Stories = await _storyRepo.GetAndSortPaginatedStoryCards(PerPage, page);

            // Prepare pagination
            Pagination = new Pagination
            {
                CurrentPage = page,
                ItemCount = storiesCount,
                PerPage = PerPage
            };
            
            return Page();
        }
    }
}
