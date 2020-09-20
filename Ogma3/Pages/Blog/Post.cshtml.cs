using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Utils.Extensions;

namespace Ogma3.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly UserRepository _userRepo;
        private readonly BlogpostsRepository _blogpostsRepo;
        
        public DetailsModel(UserRepository userRepo, BlogpostsRepository blogpostsRepo)
        {
            _userRepo = userRepo;
            _blogpostsRepo = blogpostsRepo;
        }

        public BlogpostDetails Blogpost { get; set; }
        public ProfileBar ProfileBar { get; set; }

        public async Task<IActionResult> OnGetAsync(long id, string? slug)
        {
            Blogpost = await _blogpostsRepo.Get(id);

            if (Blogpost == null) return NotFound();
            if (!Blogpost.IsPublished && !User.IsUserSameAsLoggedIn(Blogpost.AuthorId)) return NotFound();
            
            ProfileBar = await _userRepo.GetProfileBar(Blogpost.AuthorId);
            
            return Page();
        }

    }
}
