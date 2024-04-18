using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Cards;
using Ogma3.Services.UserService;

namespace Ogma3.Data.Stories;

public class StoriesRepository(ApplicationDbContext context, IMapper mapper, IUserService userService)
{
	/// <summary>
	/// Get `StoryCard` objects, sorted according to `EStorySortingOptions` and paginated
	/// </summary>
	/// <param name="count">Number of objects</param>
	/// <param name="sort">Sorting method</param>
	/// <returns>Sorted and paginated list of `StoryCard` objects</returns>
	public async Task<List<StoryCard>> GetTopStoryCards(int count, EStorySortingOptions sort = EStorySortingOptions.DateDescending)
	{
		return await context.Stories
			.TagWith($"{nameof(StoriesRepository)}.{nameof(GetTopStoryCards)} -> {count}, {sort}")
			.Where(b => b.PublicationDate != null)
			.Where(b => b.ContentBlockId == null)
			.Blacklist(context, userService.User?.GetNumericId())
			.SortByEnum(sort)
			.Take(count)
			.ProjectTo<StoryCard>(mapper.ConfigurationProvider)
			.AsNoTracking()
			.ToListAsync();
	}
}