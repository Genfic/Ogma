using AutoMapper;
using Ogma3.Data.Clubs;

namespace Ogma3.Pages.Shared.Bars;

public class ClubBar
{
	public required long Id { get; set; }
	public required string Name { get; set; }
	public required string Slug { get; set; }
	public required string Hook { get; set; }
	public required string Description { get; set; }
	public required string Icon { get; set; }
	public required DateTime CreationDate { get; set; }
	public required int ClubMembersCount { get; set; }
	public required int ThreadsCount { get; set; }
	public required int StoriesCount { get; set; }
	public bool IsMember => Role is not null;
	public required long FounderId { get; set; }
	public required EClubMemberRoles? Role { get; set; }

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			long? currentUser = null;

			CreateMap<Data.Clubs.Club, ClubBar>()
				.ForMember(cb => cb.FounderId, opts
					=> opts.MapFrom(c => c.ClubMembers.First(cm => cm.Role == EClubMemberRoles.Founder).MemberId)
				)
				.ForMember(cb => cb.StoriesCount, opts
					=> opts.MapFrom(c => c.Folders.Sum(f => f.StoriesCount))
				)
				.ForMember(cb => cb.Role, opts
					=> opts.MapFrom(c => c.ClubMembers.Any(cm => cm.MemberId == currentUser)
						? c.ClubMembers.First(cm => cm.MemberId == currentUser).Role
						: (EClubMemberRoles?)null)
				);
		}
	}
}