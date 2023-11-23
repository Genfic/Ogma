using System;
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
using Ogma3.Data.Blogposts;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Minimals;
using Utils.Extensions;

namespace Ogma3.Pages.Blog;

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

	[BindProperty] public PostData Input { get; set; } = null!;

	public async Task<IActionResult> OnGetAsync(int id)
	{
		// Get logged in user
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		// Get post and make sure the user matches
		var input = await _context.Blogposts
			.Where(m => m.Id == id)
			.Where(b => b.AuthorId == uid)
			.ProjectTo<PostData>(_mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (input is null) return NotFound();

		Input = input;
		Input.Published = Input.PublicationDate is not null;

		return Page();
	}

	public class PostData
	{
		public long Id { get; set; }
		public string Title { get; set; } = null!;
		public string Body { get; set; } = null!;
		public string? Tags { get; set; }
		public long AuthorId { get; set; }
		public ChapterMinimal? AttachedChapter { get; set; }
		public StoryMinimal? AttachedStory { get; set; }
		public bool IsUnavailable { get; set; }
		public bool Published { get; set; }
		public DateTime? PublicationDate { get; set; }
	}

	public class PostDataValidation : AbstractValidator<PostData>
	{
		public PostDataValidation()
		{
			RuleFor(b => b.Title)
				.NotEmpty()
				.Length(CTConfig.CBlogpost.MinTitleLength, CTConfig.CBlogpost.MaxTitleLength);
			RuleFor(b => b.Body)
				.NotEmpty()
				.Length(CTConfig.CBlogpost.MinBodyLength, CTConfig.CBlogpost.MaxBodyLength);
			RuleFor(b => b.Tags)
				.HashtagsFewerThan(CTConfig.CBlogpost.MaxTagsAmount)
				.HashtagsShorterThan(CTConfig.CBlogpost.MaxTagLength);
		}
	}

	public class MappingProfile : Profile
	{
		public MappingProfile() => CreateMap<Blogpost, PostData>()
			.ForMember(pd => pd.Tags, opts
				=> opts.MapFrom(b => string.Join(", ", b.Hashtags)));
	}

	public async Task<IActionResult> OnPostAsync(long id)
	{
		if (!ModelState.IsValid) return Page();

		// Get logged in user
		var uid = User.GetNumericId();
		if (uid is null) return Unauthorized();

		var post = await _context.Blogposts
			.Where(b => b.Id == id)
			.FirstOrDefaultAsync();

		if (post is null) return NotFound();
		if (post.AuthorId != uid) return Unauthorized();

		post.Title = Input.Title.Trim();
		post.Slug = Input.Title.Trim().Friendlify();
		post.Body = Input.Body.Trim();
		post.WordCount = Input.Body.Words();
		post.Hashtags = Input.Tags?.ParseHashtags() ?? Array.Empty<string>();
		post.PublicationDate = Input.Published ? DateTime.Now : null;

		await _context.SaveChangesAsync();

		return RedirectToPage("./Post", new { id, slug = Input.Title.Trim().Friendlify() });
	}
}