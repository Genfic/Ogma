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
using Ogma3.Data.Subscriptions;

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

	public List<UserRole>? UserRoles { get; set; }

	public List<Story> Stories { get; set; } = [];

	public List<Blogpost> Blogposts { get; set; } = [];

	public List<OgmaRole> Roles { get; set; } = [];

	// Blacklist
	public List<BlacklistedRating> BlacklistedRatings { get; set; } = [];
	public List<BlacklistedTag> BlacklistedTags { get; set; } = [];
	public List<OgmaUser> Blockers { get; set; } = [];
	public List<OgmaUser> Blocking { get; set; } = [];

	// Follows
	public List<OgmaUser> Followers { get; set; } = [];
	public List<OgmaUser> Following { get; set; } = [];

	// Subscriptions
	public List<CommentThread> SubscribedThreads { get; set; } = [];

	public List<Report> Reports { get; set; } = [];
	public List<Infraction> Infractions { get; set; } = [];

	public List<Notification> Notifications { get; set; } = [];

	public Subscription? Subscription { get; set; }
	public long? SubscriptionId { get; set; }
}