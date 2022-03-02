using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Club;

public class ManageUsers : PageModel
{
    private readonly ApplicationDbContext _context;
    public ManageUsers(ApplicationDbContext context) => _context = context;

    public ClubData Club { get; set; }
    public IEnumerable<UserDto> Users { get; set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        if (User.GetNumericId() is not {} uid) return Unauthorized();
        
        Club = await _context.Clubs
            .Where(c => c.Id == id)
            .Select(c => new ClubData(
                c.Id,
                c.Name,
                c.Slug,
                c.ClubMembers.FirstOrDefault(cm => cm.MemberId == uid).Role
                ))
            .FirstOrDefaultAsync();
        if (Club is null) return NotFound();

        var isPrivileged = Club.Role != null && Club.Role != EClubMemberRoles.User;

        Users = await _context.ClubMembers
            .Where(cm => cm.ClubId == id)
            .Where(cm => cm.Member.ClubsBannedFrom.All(c => c.Id != id) || isPrivileged)
            .Select(cm => new UserDto(
                cm.Member.UserName,
                cm.MemberSince,
                cm.Member.Avatar,
                cm.Role,
                cm.Member.ClubsBannedFrom.Any(c => c.Id == id)
            ))
            .ToListAsync();

        return Page();
    }


    public sealed record UserDto(
        string Name,
        DateTime JoinDate,
        string Avatar,
        EClubMemberRoles Role,
        bool IsBanned
    );

    public sealed record ClubData(
        long Id,
        string Name,
        string Slug,
        EClubMemberRoles? Role
    );
}