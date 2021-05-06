using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public IndexModel(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public UserProfileDto UserData { get; private set; }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            var uid = User.GetNumericId();
            
            UserData = await _context.Users
                .Where(u => u.NormalizedUserName == name.Normalize().ToUpper())
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider, new { currentUser = uid })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (UserData is null) return NotFound();

            return Page();
        }

    }
}