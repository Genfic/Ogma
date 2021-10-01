using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Club;

public class Members : PageModel
{
    private readonly ClubRepository _clubRepo;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Members(ClubRepository clubRepo, ApplicationDbContext context, IMapper mapper)
    {
        _clubRepo = clubRepo;
        _context = context;
        _mapper = mapper;
    }
        
    public ClubBar ClubBar { get; private set; }
    public List<UserCard> ClubMembers { get; private set; }
        
    public async Task<IActionResult> OnGetAsync(long id)
    {
        ClubBar = await _clubRepo.GetClubBar(id);
        if (ClubBar is null) return NotFound();

        ClubMembers = await _context.ClubMembers
            .Where(cm => cm.ClubId == id)
            .Select(cm => cm.Member)
            .Paginate(1, 50)
            .ProjectTo<UserCard>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        return Page();

    }
}