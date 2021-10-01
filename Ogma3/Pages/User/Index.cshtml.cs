using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Users;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.User;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepo;
    private readonly IMapper _mapper;

    public IndexModel(ApplicationDbContext context, UserRepository userRepo, IMapper mapper)
    {
        _context = context;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public ProfileBar ProfileBar { get; private set; }
    public ProfileDetails Data { get; private set; }
        
    public class ProfileDetails
    {
        public string Bio { get; init; }
        public CommentsThreadDto CommentsThread { get; init; }
    }
        
    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<OgmaUser, ProfileDetails>();
    }
        
    public async Task<IActionResult> OnGetAsync(string name)
    {
        Data = await _context.Users
            .Where(u => u.NormalizedUserName == name.Normalize().ToUpperInvariant())
            .ProjectTo<ProfileDetails>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (Data is null) return NotFound();

        Data.CommentsThread.Type = nameof(OgmaUser);
            
        ProfileBar = await _userRepo.GetProfileBar(name.ToUpper());
        if (ProfileBar is null) return NotFound();

        return Page();
    }

}