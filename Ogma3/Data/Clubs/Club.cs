using Ogma3.Data.Bases;
using Ogma3.Data.ClubThreads;
using Ogma3.Data.Folders;
using Ogma3.Data.Reports;
using Ogma3.Data.Users;

namespace Ogma3.Data.Clubs;

public class Club : BaseModel, IReportableContent
{
	public required string Name { get; set; }
	public required string Slug { get; set; }
	public required string Hook { get; set; }
	public required string Description { get; set; }
	public required string Icon { get; set; }
	public required string? IconId { get; set; }
	public DateTime CreationDate { get; set; }
	public ICollection<ClubMember> ClubMembers { get; set; } = [];
	public ICollection<OgmaUser> BannedUsers { get; set; } = [];
	public ICollection<ClubThread> Threads { get; set; } = [];
	public ICollection<Folder> Folders { get; set; } = [];
	public ICollection<Report> Reports { get; set; } = [];
}