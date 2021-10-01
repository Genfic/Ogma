using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public class StaffModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StaffModel(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public ICollection<UserCard> Staff { get; private set; }
        
    public async Task OnGetAsync()
    {
        Staff =  await _context.Users
            .Where(u => u.Roles.Any(ur => ur.IsStaff))
            .OrderBy(uc => uc.Roles.OrderBy(r => r.Order).First().Order)
            .ProjectTo<UserCard>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}