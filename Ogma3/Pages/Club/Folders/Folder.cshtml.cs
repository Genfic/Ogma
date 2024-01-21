using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Clubs;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Pages.Shared.Minimals;

namespace Ogma3.Pages.Club.Folders;

public class FolderModel(ApplicationDbContext context, IMapper mapper, OgmaConfig config, ClubRepository clubRepo) : PageModel
{
	public class FolderDetails
	{
		public required long Id { get; init; }
		public required string Name { get; init; }
		public required string Slug { get; init; }
		public required string? Description { get; init; }
		public required IEnumerable<FolderMinimal> ChildFolders { get; init; }
		public required int StoriesCount { get; init; }
		public required EClubMemberRoles AccessLevel { get; init; }
	}

	public required ClubBar ClubBar { get; set; }
	public required FolderDetails Folder { get; set; }
	public required bool EditPermitted { get; set; }
	public required List<StoryCard> Stories { get; set; }
	public required Pagination Pagination { get; set; }

	public async Task<IActionResult> OnGetAsync(long clubId, long id, [FromQuery] int page = 1)
	{
		var uid = User.GetNumericId();

		var clubBar = await clubRepo.GetClubBar(clubId);
		if (clubBar is null) return NotFound();
		ClubBar = clubBar;

		var folder = await context.Folders
			.Where(f => f.Id == id)
			.Select(f => new FolderDetails
			{
				Id = f.Id,
				Name = f.Name,
				Slug = f.Slug,
				Description = f.Description,
				StoriesCount = f.StoriesCount,
				AccessLevel = f.AccessLevel,
				ChildFolders = f.ChildFolders.Select(cf => new FolderMinimal
				{
					Id = cf.Id,
					ClubId = cf.ClubId,
					Name = cf.Name,
					Slug = cf.Slug,
					StoriesCount = cf.StoriesCount
				})
			})
			.FirstOrDefaultAsync();
		if (folder is null) return NotFound();

		Folder = folder;

		EditPermitted = await clubRepo.CheckRoles(ClubBar.Id, uid, EClubMemberRoles.Founder, EClubMemberRoles.Admin);

		Stories = await context.FolderStories
			.Where(s => s.FolderId == id)
			.Select(s => s.Story)
			.Where(s => s.PublicationDate != null)
			.Where(s => s.ContentBlockId == null)
			.Blacklist(context, uid)
			.OrderByDescending(s => s.PublicationDate)
			.Paginate(page, config.StoriesPerPage)
			.ProjectTo<StoryCard>(mapper.ConfigurationProvider)
			.ToListAsync();

		// Prepare pagination
		Pagination = new Pagination
		{
			PerPage = config.StoriesPerPage,
			ItemCount = Folder.StoriesCount,
			CurrentPage = page
		};

		return Page();
	}
}