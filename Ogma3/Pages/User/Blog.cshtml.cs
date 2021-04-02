using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User
{
    public class BlogModel : PageModel
    {
        private const int PerPage = 25;
        
        private readonly UserRepository _userRepo;
        private readonly BlogpostsRepository _blogpostsRepo;
        public BlogModel(UserRepository userRepo, BlogpostsRepository blogpostsRepo)
        {
            _userRepo = userRepo;
            _blogpostsRepo = blogpostsRepo;
        }
        
        public ICollection<BlogpostCard> Posts { get;set; }
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
                Posts = await _blogpostsRepo.GetAllPaginatedCardsForUser(name, page, PerPage);
                count = await _blogpostsRepo.CountAllForUser(name);
            }
            else
            {
                Posts = await _blogpostsRepo.GetPublicPaginatedCardsForUser(name, page, PerPage);
                count = await _blogpostsRepo.CountPublicForUser(name);
            }

            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = count,
                CurrentPage = page
            };
            
            return Page();
        }

    }
}
