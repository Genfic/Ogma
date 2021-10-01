using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Tags;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages;

public class TagModel : PageModel
{
    private const int PerPage = 25;
        
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
        
    public TagModel(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public record TagInfo(string Name, ETagNamespace? Namespace);
        
    public TagInfo Tag { get; private set; }
    public IList<StoryCard> Stories { get; private set; }
    public Pagination Pagination { get; private set; }

    public async Task<IActionResult> OnGetAsync(long id, string? slug, [FromQuery] int page = 1)
    {
        var uid = User.GetNumericId();
            
        Tag = await _context.Tags
            .Where(t => t.Id == id)
            .Select(t => new TagInfo(t.Name, t.Namespace))
            .FirstOrDefaultAsync();
            
        if (Tag is null) return NotFound();

        var query = _context.Stories
            .Where(s => s.PublicationDate != null)
            .Where(s => s.ContentBlockId == null)
            .Where(s => s.Tags.Any(st => st.Id == id))
            .Blacklist(_context, uid);
            
        Stories = await query
            .OrderByDescending(s => s.PublicationDate)
            .Paginate(page, PerPage)
            .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        // Prepare pagination
        Pagination = new Pagination
        {
            CurrentPage = page,
            ItemCount = await query.CountAsync(),
            PerPage = PerPage
        };
            
        return Page();
    }
}