using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Chapters;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;

namespace Ogma3.Pages;

public class ChapterModel : PageModel
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public ChapterModel(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public ChapterDetails? Chapter { get; private set; }

	public class ChapterDetails
	{
		public long StoryId { get; init; }
		public string StoryTitle { get; init; } = null!;
		public string StorySlug { get; init; } = null!;
		public long StoryAuthorId { get; init; }
		public string StoryRatingName { get; init; } = null!;
		public long Id { get; init; }
		public string Title { get; init; } = null!;
		public string Slug { get; init; } = null!;
		public uint Order { get; init; }
		public DateTime? PublicationDate { get; init; }
		public string Body { get; init; } = null!;
		public string? StartNotes { get; init; }
		public string? EndNotes { get; init; }
		public CommentsThreadDto CommentsThread { get; init; } = null!;
		public ChapterMicroDto? Previous { get; set; }
		public ChapterMicroDto? Next { get; set; }
		public ContentBlockCard? ContentBlock { get; set; }
	}

	public class ChapterMicroDto
	{
		public long Id { get; init; }
		public string Title { get; init; } = null!;
		public string Slug { get; init; } = null!;

		public uint Order { get; init; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Chapter, ChapterDetails>();
			CreateMap<Chapter, ChapterMicroDto>();
		}
	}

	public async Task<IActionResult> OnGetAsync(long sid, long id, string? slug)
	{
		var uid = User.GetNumericId();
		
		Chapter = await _context.Chapters
			.Where(c => c.Id == id)
			.Where(c => c.PublicationDate != null || c.Story.AuthorId == uid)
			.Where(c => c.ContentBlockId == null || c.Story.AuthorId == uid || User.IsStaff())
			.Select(MapChapterDetails)
			.FirstOrDefaultAsync();

		if (Chapter is null) return NotFound();

		Chapter.Previous = await _context.Chapters
			.Where(c => c.StoryId == Chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.Where(c => c.ContentBlockId == null)
			.Where(c => c.Order < Chapter.Order)
			.OrderBy(c => c.Order)
			.ProjectTo<ChapterMicroDto>(_mapper.ConfigurationProvider)
			.LastOrDefaultAsync();
		Chapter.Next = await _context.Chapters
			.Where(c => c.StoryId == Chapter.StoryId)
			.Where(c => c.PublicationDate != null)
			.Where(c => c.ContentBlockId == null)
			.Where(c => c.Order > Chapter.Order)
			.OrderBy(c => c.Order)
			.ProjectTo<ChapterMicroDto>(_mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		Chapter.CommentsThread.Type = nameof(Data.Chapters.Chapter);

		return Page();
	}

	private static Expression<Func<Chapter, ChapterDetails>> MapChapterDetails => c => new ChapterDetails
	{
		Id = c.Id,
		Title = c.Title,
		Slug = c.Slug,
		Order = c.Order,
		Body = c.Body,
		StartNotes = c.StartNotes,
		EndNotes = c.EndNotes,
		StoryRatingName = c.Story.Rating.Name,
		PublicationDate = c.PublicationDate,
		StoryId = c.StoryId,
		StoryTitle = c.Story.Title,
		StorySlug = c.Story.Slug,
		StoryAuthorId = c.Story.AuthorId,
		CommentsThread = new CommentsThreadDto
		{
			Id = c.CommentsThread.Id,
			LockDate = c.CommentsThread.LockDate,
			Type = nameof(Data.Chapters.Chapter)
		},
		ContentBlock = c.ContentBlock == null
			? null
			: new ContentBlockCard
			{
				Reason = c.ContentBlock.Reason,
				DateTime = c.ContentBlock.DateTime,
				IssuerUserName = c.ContentBlock.Issuer.UserName
			}
	};
}