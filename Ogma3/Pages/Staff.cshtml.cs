using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Repositories;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages
{
    public class StaffModel : PageModel
    {
        private readonly UserRepository _userRepo;

        public StaffModel(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public ICollection<UserCard> Staff { get; set; }
        
        public async Task OnGetAsync()
        {
            Staff = await _userRepo.GetStaff();
        }
    }
}