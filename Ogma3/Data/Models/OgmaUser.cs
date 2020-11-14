#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class OgmaUser : IdentityUser<long>
    {
        [PersonalData]
        [MaxLength(CTConfig.CUser.MaxTitleLength)]
        public string? Title { get; set; }
        
        [PersonalData]
        [MaxLength(CTConfig.CUser.MaxBioLength)]
        public string? Bio { get; set; }

        [PersonalData]
        public string? Avatar { get; set; }
        public string? AvatarId { get; set; }

        [Required]
        [PersonalData]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        
        [Required]
        [PersonalData]
        public DateTime LastActive { get; set; } = DateTime.Now;

        public CommentsThread CommentsThread { get; set; } = new CommentsThread();
        
        [JsonIgnore]
        public ICollection<UserRole>? UserRoles { get; set; }
        
        [JsonIgnore] 
        public ICollection<Story> Stories { get; set; } = new List<Story>();

        [JsonIgnore]
        public ICollection<Blogpost> Blogposts { get; set; } = new List<Blogpost>();

        [JsonIgnore]
        public ICollection<OgmaRole> Roles { get; set; }

        // Blacklist
        [JsonIgnore]
        public ICollection<BlacklistedRating> BlacklistedRatings { get; set; }
        [JsonIgnore]
        public ICollection<BlacklistedTag> BlacklistedTags { get; set; }
        // [JsonIgnore]
        public ICollection<OgmaUser> BlockedUsers { get; set; }
        // [JsonIgnore]
        public ICollection<OgmaUser> BlockedByUsers { get; set; }
        
        // Follows
        // [JsonIgnore]
        public ICollection<OgmaUser> Followers { get; set; }
        // [JsonIgnore]
        public ICollection<OgmaUser> Following { get; set; }

        public bool IsLoggedIn(ClaimsPrincipal claimsPrincipal)
        {
            return Id.ToString() == claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}    