using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.ClubModeratorActions;
using Ogma3.Data.Clubs;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;

namespace Ogma3.Pages.Club;

public class Modlog : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ClubRepository _clubRepo;
        
    private const int PerPage = 50;

    public Modlog(ApplicationDbContext context, ClubRepository clubRepo)
    {
        _context = context;
        _clubRepo = clubRepo;
    }

    public ICollection<ClubModeratorAction> Actions { get; private set; }
    public ClubBar ClubBar { get; private set; }
    public Pagination Pagination { get; private set; }

    public async Task<ActionResult> OnGetAsync(long id, [FromQuery] int page = 1)
    {
        ClubBar = await _clubRepo.GetClubBar(id);
        if (ClubBar is null) return NotFound();

        var query = _context.ClubModeratorActions
            .Where(cma => cma.ClubId == id);

        Actions = await query
            .OrderByDescending(ma => ma.CreationDate)
            .Paginate(page, PerPage)
            .ToListAsync();
            
        Pagination = new Pagination
        {
            PerPage = PerPage,
            CurrentPage = page,
            ItemCount = await query.CountAsync()
        };

        return Page();
    }
}