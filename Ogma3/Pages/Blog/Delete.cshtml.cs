using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Blog;

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

	public class GetData
	{
		public long Id { get; init; }
		public long AuthorId { get; set; }
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
			.FirstOrDefaultAsync();

		if (Blogpost is null) return NotFound();
		if (Blogpost.AuthorId != User.GetNumericId()) return Unauthorized();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(int? id)
	{
		if (id == null) return NotFound();

		// Get logged in user
		var uname = User.GetUsername();
		if (uname is null) return Unauthorized();

		var blogpost = await _context.Blogposts
			.Where(b => b.Id == id)
			.FirstOrDefaultAsync();

		if (blogpost is null) return NotFound();
		if (blogpost.AuthorId != User.GetNumericId()) return Unauthorized();

		_context.Blogposts.Remove(blogpost);

		await _context.SaveChangesAsync();

		return RedirectToPage("/User/Blog", new { name = uname });
	}
}