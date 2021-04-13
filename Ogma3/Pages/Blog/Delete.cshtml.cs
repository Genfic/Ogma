using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Blog
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DeleteModel(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public GetData Blogpost { get; set; }

        public record GetData
        {
            public long Id { get; init; }
            public string Title { get; init; }
            public string Slug { get; init; }
            public DateTime PublishDate { get; init; }
            public bool IsPublished { get; init; }
            public int CommentsThreadCommentsCount { get; init; }
        }
        
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Blogpost, GetData>();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Blogpost = await _context.Blogposts
                .Where(m => m.Id == id)
                .ProjectTo<GetData>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsyncEF();

            if (Blogpost == null) return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();
            
            // Get logged in user
            var uid = User.GetNumericId();
            var uname = User.GetUsername();

            if (uid == null || uname == null) return Unauthorized();
     
            await _context.Blogposts
                .Where(b => b.Id == id && b.AuthorId == uid)
                .DeleteAsync();

            await _context.SaveChangesAsync();

            return RedirectToPage("/User/Blog", new { name = uname });
        }
    }
}
