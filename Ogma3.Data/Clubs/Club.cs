using AutoDbSetGenerators;
using Ogma3.Data.Bases;
using Ogma3.Data.ClubThreads;
using Ogma3.Data.Folders;
using Ogma3.Data.Images;
using Ogma3.Data.Reports;

namespace Ogma3.Data.Clubs;

[AutoDbSet]
public sealed class Club : BaseModel, IReportableContent
{
	public required string Name { get; set; }
	public required string Slug { get; set; }
	public required string Hook { get; set; }
	public required string Description { get; set; }
	public required Image Icon { get; set; }
	public long IconId { get; set; }
	public DateTimeOffset CreationDate { get; set; }
	public List<ClubMember> ClubMembers { get; set; } = [];
	public List<ClubThread> Threads { get; set; } = [];
	public List<Folder> Folders { get; set; } = [];
	public List<Report> Reports { get; set; } = [];
}