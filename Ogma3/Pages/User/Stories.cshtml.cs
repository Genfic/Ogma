using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
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
            
            Stories = await _storyRepo.GetAndSortPaginatedStoryCards(PerPage, page, publishedOnly: !isCurrentUser);
            
            // Prepare pagination
            Pagination = new Pagination
            {
                CurrentPage = page,
                ItemCount = await _storyRepo.CountForUser(ProfileBar.Id, !isCurrentUser),
                PerPage = PerPage
            };
            
            return Page();
        }
    }
}
