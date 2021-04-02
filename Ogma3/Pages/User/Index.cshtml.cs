using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ogma3.Data.Users;

namespace Ogma3.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly UserRepository _userRepo;

        public IndexModel(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public UserProfileDto UserData { get; set; }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            UserData = await _userRepo.GetUserData(name);

            if (UserData == null) return NotFound();

            return Page();
        }

    }
}