#nullable enable
using System;
using System.Collections.Generic;
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

public class OgmaUser : IdentityUser<long>, IReportableContent
{
    [PersonalData]
    public string? Title { get; set; }
        
    [PersonalData]
    public string? Bio { get; set; }

    [PersonalData] 
    public List<string> Links { get; set; } = new();

    [PersonalData]
    public string Avatar { get; set; } = null!;
    public string? AvatarId { get; set; }

    [PersonalData]
    public DateTime RegistrationDate { get; set; }
        
    [PersonalData]
    public DateTime LastActive { get; set; }

    public CommentsThread CommentsThread { get; set; } = new();
        
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
    public ICollection<CommentsThread> SubscribedThreads { get; set; } = null!;
        
    public ICollection<Report> Reports { get; set; } = null!;
    public ICollection<Infraction> Infractions { get; set; } = null!;
    public ICollection<Notification> Notifications { get; set; } = null!;
}