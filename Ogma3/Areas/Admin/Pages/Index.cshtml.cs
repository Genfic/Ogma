using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Areas.Admin.Models;
using Ogma3.Data;

namespace Ogma3.Areas.Admin.Pages;

public class Index : PageModel
{
    private readonly ApplicationDbContext _context;
    public Index(ApplicationDbContext context) => _context = context;

    public Dictionary<string, int> Counts { get; private set; }
    public List<TableInfo> DbSizes { get; private set; }
    
    public async Task OnGet()
    {
        Counts = new Dictionary<string, int>
        {
            ["Stories"] = await _context.Stories.CountAsync(),
            ["Chapters"] = await _context.Chapters.CountAsync(),
            ["Blogposts"] = await _context.Blogposts.CountAsync(),
            ["Users"] = await _context.Users.CountAsync(),
            ["Comments"] = await _context.Comments.CountAsync(),
            ["Reports"] = await _context.Reports.CountAsync(),
        };
        
        DbSizes = await _context.TableInfos.FromSqlRaw(@"
            SELECT 
                table_name as name, 
                pg_relation_size(quote_ident(table_name)) as size
            FROM information_schema.tables 
            WHERE table_schema = 'public' AND table_name NOT LIKE '\_\_%'
            ORDER BY size DESC
        ").ToListAsync();

    }

}