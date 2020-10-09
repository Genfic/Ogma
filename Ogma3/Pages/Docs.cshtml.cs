using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.DTOs;
using Utils.Extensions;

namespace Ogma3.Pages
{
    public class Docs : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Docs(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public DocumentDto Document { get; set; }
        public List<DocumentVersionDto> Versions { get; set; }
        public IEnumerable<String.Header> Headers { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string slug, [FromQuery] uint? v)
        {
            var query = _context.Documents
                .Where(d => d.Slug == slug);

            query = v.HasValue 
                ? query.Where(d => d.Version == v) 
                : query.OrderByDescending(d => d.Version);
            
            Document = await query
                .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (Document == null)
                return NotFound();
            
            Versions = await _context.Documents
                .Where(d => d.Slug == Document.Slug)
                .OrderByDescending(d => d.Version)
                .ProjectTo<DocumentVersionDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            Headers = Document.Body.GetMarkdownHeaders();
            
            return Page();
        }
    }
}