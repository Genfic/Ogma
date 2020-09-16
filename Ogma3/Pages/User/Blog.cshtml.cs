using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;
using Utils.Extensions;

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
            
            Posts = await _blogpostsRepo.GetPaginatedCardsForUser(name, page, PerPage, !isCurrentUser);
            
            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = await _blogpostsRepo.CountForUser(name, !isCurrentUser),
                CurrentPage = page
            };
            
            return Page();
        }

    }
}
