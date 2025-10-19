using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Blogposts;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Images;
using Ogma3.Data.Infractions;
using Ogma3.Data.Notifications;
using Ogma3.Data.Reports;
using Ogma3.Data.Roles;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Users;

public sealed class OgmaUser : IdentityUser<long>, IReportableContent
{
	// IdentityUser overrides
	public new string UserName {
		get => base.UserName ?? "";
		set => base.UserName = value;
	}
	public new string NormalizedUserName
	{
		get => base.NormalizedUserName ?? "";
		set => base.NormalizedUserName = value;
	}
	public new string Email
	{
		get => base.Email ?? "";
		set => base.Email = value;
	}
	public new string NormalizedEmail
	{
		get => base.NormalizedEmail ?? "";
		set => base.NormalizedEmail = value;
	}

	// New properties
	[PersonalData] public string? Title { get; set; }

	[PersonalData] public string? Bio { get; set; }

	[PersonalData] public List<string> Links { get; set; } = [];

	public Image Avatar { get; set; } = null!;
	public long AvatarId { get; set; }

	[PersonalData] public DateTimeOffset RegistrationDate { get; set; }

	[PersonalData] public DateTimeOffset LastActive { get; set; }

	public DateTimeOffset? DeletedAt { get; set; }

	public CommentThread CommentThread { get; set; } = new();

	public ICollection<UserRole>? UserRoles { get; set; }

	public ICollection<Story> Stories { get; set; } = [];

	public ICollection<Blogpost> Blogposts { get; set; } = [];

	public ICollection<OgmaRole> Roles { get; set; } = [];

	// Blacklist
	public ICollection<BlacklistedRating> BlacklistedRatings { get; set; } = [];
	public ICollection<BlacklistedTag> BlacklistedTags { get; set; } = [];
	public ICollection<OgmaUser> Blockers { get; set; } = [];
	public ICollection<OgmaUser> Blocking { get; set; } = [];

	// Follows
	public ICollection<OgmaUser> Followers { get; set; } = [];
	public ICollection<OgmaUser> Following { get; set; } = [];

	// Subscriptions
	public ICollection<CommentThread> SubscribedThreads { get; set; } = [];

	public ICollection<Report> Reports { get; set; } = [];
	public ICollection<Infraction> Infractions { get; set; } = [];

	public ICollection<Notification> Notifications { get; set; } = [];
}