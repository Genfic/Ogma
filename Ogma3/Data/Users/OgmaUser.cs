using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Blogposts;
using Ogma3.Data.CommentsThreads;
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

	[PersonalData] public string Avatar { get; set; } = null!;
	public string? AvatarId { get; set; }

	[PersonalData] public DateTimeOffset RegistrationDate { get; set; }

	[PersonalData] public DateTimeOffset LastActive { get; set; }

	public DateTimeOffset? DeletedAt { get; set; }

	public CommentThread CommentThread { get; set; } = new();

	public ICollection<UserRole>? UserRoles { get; set; }

	public ICollection<Story> Stories { get; set; } = null!;

	public ICollection<Blogpost> Blogposts { get; set; } = null!;

	public ICollection<OgmaRole> Roles { get; set; } = null!;

	// Blacklist
	public ICollection<BlacklistedRating> BlacklistedRatings { get; set; } = null!;
	public ICollection<BlacklistedTag> BlacklistedTags { get; set; } = null!;
	public ICollection<OgmaUser> Blockers { get; set; } = null!;
	public ICollection<OgmaUser> Blocking { get; set; } = null!;

	// Follows
	public ICollection<OgmaUser> Followers { get; set; } = null!;
	public ICollection<OgmaUser> Following { get; set; } = null!;

	// Subscriptions
	public ICollection<CommentThread> SubscribedThreads { get; set; } = null!;

	public ICollection<Report> Reports { get; set; } = null!;
	public ICollection<Infraction> Infractions { get; set; } = null!;

	public ICollection<Notification> Notifications { get; set; } = null!;
}