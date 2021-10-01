using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Data.Roles;
using Ogma3.Data.Users;

namespace Ogma3.Areas.Admin.Pages;

public class Users : PageModel
{
    private readonly ApplicationDbContext _context;

    public Users(ApplicationDbContext context)
    {
        _context = context;
    }

    public UserDetailsDto? OgmaUser { get; set; }
    public List<OgmaRole> Roles { get; set; }
        
    public async Task<ActionResult> OnGet([FromQuery] string? name)
    {
        if (string.IsNullOrEmpty(name)) return Page();
            
        OgmaUser = await _context.Users
            .Where(u => u.NormalizedUserName == name.ToUpper())
            .Select(u => new UserDetailsDto
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                Title = u.Title,
                Avatar = u.Avatar,
                Bio = u.Bio,
                RoleNames = u.Roles.Select(r => r.Name),
                RegistrationDate = u.RegistrationDate,
                LastActive = u.LastActive,
                StoriesCount = u.Stories.Count,
                BlogpostsCount = u.Blogposts.Count,
                BannedUntil = u.Infractions
                    .Where(i => i.Type == InfractionType.Ban)
                    .Select(i => i.ActiveUntil)
                    .FirstOrDefault(),
                MutedUntil = u.Infractions
                    .Where(i => i.Type == InfractionType.Mute)
                    .Select(i => i.ActiveUntil)
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();
        if (OgmaUser is null) return NotFound();

        Roles = await _context.Roles.ToListAsync();

        return Page();
    }
}