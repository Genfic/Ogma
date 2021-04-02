using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Blacklists;
using Ogma3.Data.Blogposts;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Notifications;
using Ogma3.Data.Reports;
using Ogma3.Data.Roles;
using Ogma3.Data.Stories;

namespace Ogma3.Data.Users
{
    public class OgmaUser : IdentityUser<long>, IReportableContent
    {
        [PersonalData]
        public string? Title { get; set; }
        
        [PersonalData]
        public string? Bio { get; set; }

        [PersonalData]
        public string? Avatar { get; set; }
        public string? AvatarId { get; set; }

        [PersonalData]
        public DateTime RegistrationDate { get; set; }
        
        [PersonalData]
        public DateTime LastActive { get; set; }

        public CommentsThread CommentsThread { get; set; } = new();
        
        [JsonIgnore]
        public ICollection<UserRole>? UserRoles { get; set; }
        
        [JsonIgnore] 
        public ICollection<Story> Stories { get; set; }

        [JsonIgnore]
        public ICollection<Blogpost> Blogposts { get; set; }

        [JsonIgnore]
        public ICollection<OgmaRole> Roles { get; set; }

        // Blacklist
        [JsonIgnore]
        public ICollection<BlacklistedRating> BlacklistedRatings { get; set; }
        [JsonIgnore]
        public ICollection<BlacklistedTag> BlacklistedTags { get; set; }
        [JsonIgnore]
        public ICollection<OgmaUser> BlockedUsers { get; set; }
        [JsonIgnore]
        public ICollection<OgmaUser> BlockedByUsers { get; set; }
        
        // Follows
        [JsonIgnore]
        public ICollection<OgmaUser> Followers { get; set; }
        [JsonIgnore]
        public ICollection<OgmaUser> Following { get; set; }
        
        // Subscriptions
        public ICollection<CommentsThread> SubscribedThreads { get; set; }
        
        // Bans and mutes
        public DateTime? BannedUntil { get; set; }
        public DateTime? MutedUntil { get; set; }

        public ICollection<Report> Reports { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        
        public bool IsLoggedIn(ClaimsPrincipal claimsPrincipal)
        {
            return Id.ToString() == claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}    