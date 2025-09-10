using Microsoft.EntityFrameworkCore;
using Ogma3.Infrastructure.Extensions;
using Ogma3.Pages.Shared.Bars;
using Ogma3.Services.UserService;

namespace Ogma3.Data.Clubs;

public sealed class ClubRepository(ApplicationDbContext context, IUserService userService)
{
	public async Task<ClubBar?> GetClubBar(long clubId)
	{
		if (userService.User?.GetNumericId() is not {} uid) return null;
		return await context.Clubs
			.TagWithCallSite()
			.Where(c => c.Id == clubId)
			.Select(c => new ClubBar
			{
				Id = c.Id,
				Name = c.Name,
				Slug = c.Slug,
				Hook = c.Hook,
				Description = c.Description,
				Icon = c.Icon.Url,
				CreationDate = c.CreationDate,
				ThreadsCount = c.Threads.Count,
				ClubMembersCount = c.ClubMembers.Count,
				StoriesCount = c.Folders.Sum(f => f.Stories.Count),
				FounderId = c.ClubMembers.First(cm => cm.Role == EClubMemberRoles.Founder).MemberId,
				Role = c.ClubMembers.Any(cm => cm.MemberId == uid)
					? c.ClubMembers.First(cm => cm.MemberId == uid).Role
					: null,
			})
			.FirstOrDefaultAsync();
	}

	public async Task<bool> CheckRoles(long clubId, long? userId, params EClubMemberRoles[] roles)
	{
		if (userId is null) return false;

		return await context.Clubs
			.TagWith($"{nameof(ClubRepository)} : {nameof(CheckRoles)} — {clubId}, {userId}")
			.Where(c => c.Id == clubId)
			.Where(c => c.ClubMembers
				.Any(cm => cm.MemberId == userId && roles.ToList().Contains(cm.Role))
			)
			.AnyAsync();
	}

	public async Task<bool> IsMember(long? userId, long clubId)
	{
		return await context.ClubMembers
			.Where(cm => cm.MemberId == userId)
			.Where(cm => cm.ClubId == clubId)
			.AnyAsync();
	}
}