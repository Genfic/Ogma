#nullable enable


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Shelves;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;

namespace Ogma3.Pages.Shelves;

public class Bookshelf : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public Bookshelf(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public BookshelfDetails Shelf { get; private set; } = null!;

	public class BookshelfDetails
	{
		public string Name { get; init; } = null!;
		public string Description { get; init; } = null!;
		public string Color { get; init; } = null!;
		public string IconName { get; init; } = null!;
		public ICollection<StoryCard> Stories { get; init; } = null!;
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Shelf, BookshelfDetails>();
	}

	public async Task<IActionResult> OnGetAsync(int id, string? slug)
	{
		var uid = User.GetNumericId();

		var shelf = await _context.Shelves
			.Where(s => s.Id == id)
			.Where(s => s.IsPublic || s.OwnerId == uid)
			.ProjectTo<BookshelfDetails>(_mapper.ConfigurationProvider)
			.AsNoTracking()
			.FirstOrDefaultAsync();

		if (shelf is null) return NotFound();

		Shelf = shelf;

		return Page();
	}
}