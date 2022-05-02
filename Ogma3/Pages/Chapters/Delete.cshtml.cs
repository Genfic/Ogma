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
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Pages.Chapters;

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

	[BindProperty] public GetData Chapter { get; set; }

	public class GetData
	{
		public long Id { get; init; }
		public long StoryAuthorId { get; init; }
		public DateTime PublishDate { get; init; }
		public bool IsPublished { get; init; }
		public string Title { get; init; }
		public string Slug { get; init; }
		public int WordCount { get; init; }
		public int CommentsThreadCommentsCount { get; init; }
		public string StoryTitle { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Chapter, GetData>();
	}

	public async Task<IActionResult> OnGetAsync(long id)
	{
		Chapter = await _context.Chapters
			.Where(c => c.Id == id)
			.ProjectTo<GetData>(_mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (Chapter is null) return NotFound();
		if (Chapter.StoryAuthorId != User.GetNumericId()) return Unauthorized();

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		// Get chapter
		var chapter = await _context.Chapters
			.Where(c => c.Id == id)
			.Include(c => c.Story)
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();
		if (chapter.Story.AuthorId != User.GetNumericId()) return Unauthorized();

		// Recalculate words and chapters in the story
		chapter.Story.WordCount -= chapter.WordCount;
		chapter.Story.ChapterCount -= 1;

		_context.Chapters.Remove(chapter);

		await _context.SaveChangesAsync();

		return RedirectToPage("../Story", new { id = chapter.StoryId });
	}
}