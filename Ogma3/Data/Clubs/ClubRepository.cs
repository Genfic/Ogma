using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Services.UserService;

namespace Ogma3.Data.Clubs;

public class ClubRepository
{
	private readonly ApplicationDbContext _context;
	private readonly long? _uid;

	public ClubRepository(ApplicationDbContext context, IUserService userService)
	{
		_context = context;
		_uid = userService.User?.GetNumericId();
	}

	public async Task<ClubBar?> GetClubBar(long clubId)
	{
		if (_uid is null) return null;
		var club = await _context.Clubs
			.TagWithCallSite()
			.Where(c => c.Id == clubId)
			.Select(c => new ClubBar
			{
				Id = c.Id,
				Name = c.Name,
				Slug = c.Slug,
				Hook = c.Hook,
				Description = c.Description,
				Icon = c.Icon,
				CreationDate = c.CreationDate,
				ThreadsCount = c.Threads.Count,
				ClubMembersCount = c.ClubMembers.Count,
				StoriesCount = c.Folders.Sum(f => f.StoriesCount),
				FounderId = c.ClubMembers.First(cm => cm.Role == EClubMemberRoles.Founder).MemberId,
				Role = c.ClubMembers.Any(cm => cm.MemberId == _uid)
					? c.ClubMembers.First(cm => cm.MemberId == _uid).Role
					: null
			})
			.FirstOrDefaultAsync();
		return club;
	}

	public async Task<bool> CheckRoles(long clubId, long? userId, params EClubMemberRoles[] roles)
	{
		if (userId is null) return false;

		return await _context.Clubs
			.TagWith($"{nameof(ClubRepository)} : {nameof(CheckRoles)} — {clubId}, {userId}")
			.Where(c => c.Id == clubId)
			.Where(c => c.ClubMembers
				.Any(cm => cm.MemberId == userId && roles.Contains(cm.Role))
			)
			.AnyAsync();
	}

	public async Task<bool> IsMember(long? userId, long clubId)
	{
		return await _context.ClubMembers
			.Where(cm => cm.MemberId == userId)
			.Where(cm => cm.ClubId == clubId)
			.AnyAsync();
	}
}