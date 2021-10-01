using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Infractions;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Areas.Identity.Pages.Account.Manage;

public class Infractions : PageModel
{
    private readonly ApplicationDbContext _context;
    public Infractions(ApplicationDbContext context) => _context = context;

    public List<InfractionDto> AllInfractions { get; private set; }
        
    public async Task<IActionResult> OnGet()
    {
        var uid = User.GetNumericId();
        if (uid is null) return Unauthorized();
            
        AllInfractions = await _context.Infractions
            .Where(i => i.UserId == uid)
            .Where(i => i.Type != InfractionType.Note)
            .OrderByDescending(i => i.Type)
            .ThenByDescending(i => i.ActiveUntil)
            .Select(i => new InfractionDto
            {
                IssueDate = i.IssueDate,
                ActiveUntil = i.ActiveUntil,
                RemovedAt = i.RemovedAt,
                Reason = i.Reason,
                Type = i.Type
            })
            .ToListAsync();

        return Page();
    }
}
    
public sealed record InfractionDto
{
    public DateTime IssueDate { get; init; }
    public DateTime ActiveUntil { get; init; }
    public DateTime? RemovedAt { get; init; }
    public string Reason { get; init; }
    public InfractionType Type { get; init; }
}