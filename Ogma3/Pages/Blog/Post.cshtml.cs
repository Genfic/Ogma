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
using Ogma3.Data.Blogposts;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Pages.Blog;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepo;
    private readonly IMapper _mapper;

    public DetailsModel(UserRepository userRepo, ApplicationDbContext context, IMapper mapper)
    {
        _userRepo = userRepo;
        _context = context;
        _mapper = mapper;
    }

    public Details Blogpost { get; private set; }
    public ProfileBar ProfileBar { get; private set; }

    public bool IsUnavailable { get; private set; }
        
    public class Details
    {
        public long Id { get; init; }
        public long AuthorId { get; init; }
        public string Title { get; init; }
        public string Slug { get; init; }
        public DateTime? PublicationDate { get; init; }
        public string Body { get; init; }
        public IEnumerable<string> Hashtags { get; init; }
        public CommentsThreadDto CommentsThread { get; init; }
        public int CommentsCount { get; init; }
        public ChapterMinimal? AttachedChapter { get; set; }
        public StoryMinimal? AttachedStory { get; set; }
        public long? ContentBlockId { get; set; }
    }
        
    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<Blogpost, Details>();
    }

    public async Task<IActionResult> OnGetAsync(long id, string? slug)
    {
        Blogpost = await _context.Blogposts
            .Where(b => b.Id == id)
            .Where(b => b.PublicationDate != null || b.AuthorId == User.GetNumericId())
            .ProjectTo<Details>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (Blogpost is null) return NotFound();

        if (Blogpost.AttachedChapter is not null && Blogpost.AttachedChapter.PublicationDate is null)
        {
            IsUnavailable = true;
        }
        else if (Blogpost.AttachedStory is not null && Blogpost.AttachedStory.PublicationDate is null)
        {
            IsUnavailable = true;
        }

        Blogpost.CommentsThread.Type = nameof(Data.Blogposts.Blogpost);
            
        ProfileBar = await _userRepo.GetProfileBar(Blogpost.AuthorId);
            
        return Page();
    }

}