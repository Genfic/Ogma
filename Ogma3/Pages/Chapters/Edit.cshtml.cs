using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Utils.Extensions;

namespace Ogma3.Pages.Chapters;

[Authorize]
public class EditModel : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public EditModel(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	[BindProperty] public PostData Input { get; set; }

	public async Task<IActionResult> OnGetAsync(long id)
	{
		// Get chapter
		Input = await _context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == User.GetNumericId())
			.ProjectTo<PostData>(_mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (Input is null) return NotFound();

		Input.IsPublished = Input.PublicationDate is not null;

		return Page();
	}

	public class PostData
	{
		public long Id { get; init; }
		public string Title { get; init; }
		public string Body { get; init; }
		[Display(Name = "Start notes")] public string StartNotes { get; init; }
		[Display(Name = "End notes")] public string EndNotes { get; init; }
		public DateTime? PublicationDate { get; set; }
		public bool IsPublished { get; set; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Chapter, PostData>();
	}

	public class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			RuleFor(b => b.Title)
				.NotEmpty()
				.Length(CTConfig.CChapter.MinTitleLength, CTConfig.CChapter.MaxTitleLength);
			RuleFor(b => b.Body)
				.NotEmpty()
				.Length(CTConfig.CChapter.MinBodyLength, CTConfig.CChapter.MaxBodyLength);
			RuleFor(c => c.StartNotes)
				.MaximumLength(CTConfig.CChapter.MaxNotesLength);
			RuleFor(c => c.EndNotes)
				.MaximumLength(CTConfig.CChapter.MaxNotesLength);
			RuleFor(c => c.IsPublished)
				.NotNull();
		}
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		if (!ModelState.IsValid) return Page();

		var chapter = await _context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.Story.AuthorId == uid)
			.Include(c => c.Story)
			.FirstOrDefaultAsync();

		if (chapter is null) return NotFound();

		// Update the chapter
		chapter.Title = Input.Title.Trim();
		chapter.Body = Input.Body.Trim();
		chapter.StartNotes = Input.StartNotes?.Trim();
		chapter.EndNotes = Input.EndNotes?.Trim();
		chapter.Slug = Input.Title.Trim().Friendlify();
		chapter.WordCount = Input.Body.Words();
		chapter.PublicationDate = Input.IsPublished ? DateTime.Now : null;
		await _context.SaveChangesAsync();

		chapter.Story.WordCount = await _context.Chapters
			.Where(c => c.StoryId == chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.SumAsync(c => c.WordCount);

		chapter.Story.ChapterCount = await _context.Chapters
			.Where(c => c.StoryId == chapter.StoryId)
			.CountAsync(c => c.PublicationDate != null);

		await _context.SaveChangesAsync();

		return RedirectToPage("../Chapter", new { id = chapter.Id, slug = chapter.Slug });
	}
}