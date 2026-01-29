using Ogma3.Infrastructure.Attributes;
using Riok.Mapperly.Abstractions;

namespace Ogma3.Infrastructure.OgmaConfig;

public sealed class OgmaConfig
{
	public string Cdn { get; set; } = "";
	public string AvatarServiceUrl { get; set; } = "";
	public string AdminEmail { get; set; } = "";
	public int MaxInvitesPerUser { get; set; }

	// Docs
	[AutoformCategory("Docs")] public string PrivacyPolicyDoc { get; set; } = "";
	[AutoformCategory("Docs")] public string AboutDoc { get; set; } = "";
	[AutoformCategory("Docs")] public string TosDoc { get; set; } = "";
	[AutoformCategory("Docs")] public string SearchHelpDoc { get; set; } = "";

	// Pagination settings
	[AutoformCategory("Pagination")] public int CommentsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int ClubsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int BlogpostsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int StoriesPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int ClubThreadsPerPage { get; set; } = 25;
	[AutoformCategory("Pagination")] public int ShelvesPerPage { get; set; } = 50;

	// Image sizes
	[AutoformCategory("Image sizes")] public int StoryCoverWidth { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int StoryCoverHeight { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int ClubIconWidth { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int ClubIconHeight { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int AvatarWidth { get; set; } = 250;
	[AutoformCategory("Image sizes")] public int AvatarHeight { get; set; } = 250;

	// Moderation
	[AutoformCategory("Moderation")] public int MinReportLength { get; set; } = 30;
}

[Mapper]
public static partial class OgmaConfigMapper
{
	public static partial void CopyTo(OgmaConfig from, OgmaConfig to);
}