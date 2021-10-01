using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Ratings;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Stories;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly OgmaConfig _config;
    private readonly IMapper _mapper;

    public IndexModel(ApplicationDbContext context, OgmaConfig config, IMapper mapper)
    {
        _context = context;
        _config = config;
        _mapper = mapper;
    }

    public List<Rating> Ratings { get; private set; }
    public List<StoryCard> Stories { get; private set; }
    public IEnumerable<long> Tags { get; private set; }
    public EStorySortingOptions SortBy { get; set; }
    public string SearchBy { get; private set; }
    public long? Rating { get; private set; }
    public Pagination Pagination { get; private set; }

    public async Task OnGetAsync(
        [FromQuery] IList<long> tags,
        [FromQuery] string q = null,
        [FromQuery] EStorySortingOptions sort = EStorySortingOptions.DateDescending,
        [FromQuery] long? rating = null,
        [FromQuery] int page = 1
    )
    {
        var uid = User.GetNumericId();

        SearchBy = q;
        SortBy = sort;
        Rating = rating;
        Tags = tags;

        // Load ratings
        Ratings = await _context.Ratings.ToListAsync();

        // Load stories
        var query = _context.Stories
            .AsQueryable()
            .Search(tags, q, rating)
            .Where(s => s.PublicationDate != null)
            .Where(s => s.ContentBlockId == null)
            .Blacklist(_context, uid);
            
        Stories = await query
            .SortByEnum(sort)
            .Paginate(page, _config.StoriesPerPage)
            .ProjectTo<StoryCard>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        // Prepare pagination
        Pagination = new Pagination
        {
            PerPage = _config.StoriesPerPage,
            ItemCount = await query.CountAsync(),
            CurrentPage = page
        };
    }
}