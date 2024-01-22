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

public class StaffModel(ApplicationDbContext context, IMapper mapper) : PageModel
{
	public required ICollection<UserCard> Staff { get; set; }

	public async Task OnGetAsync()
	{
		Staff = await context.Users
			.Where(u => u.Roles.Any(ur => ur.IsStaff))
			.OrderBy(uc => uc.Roles.OrderBy(r => r.Order).First().Order)
			.ProjectTo<UserCard>(mapper.ConfigurationProvider)
			.ToListAsync();
	}
}