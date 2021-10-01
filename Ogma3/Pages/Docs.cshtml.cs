using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Documents;
using Utils.Extensions;
using String = Utils.Extensions.String;

namespace Ogma3.Pages;

public class Docs : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Docs(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public DocumentDto Document { get; private set; }
    public List<DocumentVersionDto> Versions { get; private set; }
    public IEnumerable<String.Header> Headers { get; private set; }
        
    public class DocumentDto
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        public uint Version { get; set; }
        public Guid GroupId { get; set; }
    }

    public class DocumentVersionDto
    {
        public long Id { get; set; }
        public string Slug { get; set; }
        public uint Version { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Document, DocumentDto>();
            CreateMap<Document, DocumentVersionDto>();
        }
    }
        
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
            
        if (Document is null) return NotFound();
            
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