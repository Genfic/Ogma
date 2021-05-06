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
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.User
{
    public class BlogModel : PageModel
    {
        private const int PerPage = 25;
        
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BlogModel(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public string Name { get; private set; }
        public bool IsOwner { get; private set; }
        public ICollection<BlogpostCard> Posts { get; private set; }
        public Pagination Pagination { get; private set; }
        
        public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page = 1)
        {
            Name = name;
            
            var uname = User.GetUsername()?.Normalize().ToUpperInvariant();
            if (uname is null) return NotFound();

            IsOwner = string.Equals(name, uname, StringComparison.InvariantCultureIgnoreCase);

            // Start building the query
            var query = _context.Blogposts
                .Where(b => b.Author.NormalizedUserName == name.Normalize().ToUpper());
            
            if (!IsOwner)
            {   // If the profile page doesn't belong to the current user, apply additional filters
                query = query
                    .Where(b => b.IsPublished)
                    .Where(b => b.ContentBlockId == null);
            }

            // Resolve query
            Posts = await query
                .OrderByDescending(b => b.PublishDate)
                .Paginate(page, PerPage)
                .ProjectTo<BlogpostCard>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            
            // Prepare pagination
            Pagination = new Pagination
            {
                PerPage = PerPage,
                ItemCount = await query.CountAsync(),
                CurrentPage = page
            };
            
            return Page();
        }

    }
}
