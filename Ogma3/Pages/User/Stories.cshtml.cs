using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;
using Utils.Extensions;

namespace Ogma3.Pages.User
{
    public class StoriesModel : PageModel
    {
        private const int PerPage = 25;
        
        private readonly UserRepository _userRepo;
        private readonly StoriesRepository _storyRepo;
        public StoriesModel(UserRepository userRepo, StoriesRepository storyRepo)
        {
            _userRepo = userRepo;
            _storyRepo = storyRepo;
        }

        public IList<StoryCard> Stories { get;set; }
        public ProfileBar ProfileBar { get; set; }
        public Pagination Pagination { get; set; }

        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            ProfileBar = await _userRepo.GetProfileBar(name.ToUpper());
            
            if (ProfileBar == null) return NotFound();

            var isCurrentUser = User.IsUserSameAsLoggedIn(ProfileBar.Id);

            int count;
            if (isCurrentUser)
            {
                Stories = await _storyRepo.GetAndSortOwnedPaginatedStoryCards(PerPage, page);
                count = await _storyRepo.CountOwnedForUser(ProfileBar.Id);
            } else
            {
                Stories = await _storyRepo.GetAndSortPaginatedStoryCards(PerPage, page);
                count = await _storyRepo.CountForUser(ProfileBar.Id);
            }

            // Prepare pagination
            Pagination = new Pagination
            {
                CurrentPage = page,
                ItemCount = count,
                PerPage = PerPage
            };
            
            return Page();
        }
    }
}
