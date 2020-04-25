#nullable enable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class User : IdentityUser<long>
    {
        [PersonalData]
        [MaxLength(CTConfig.User.MaxTitleLength)]
        public string? Title { get; set; }
        
        [PersonalData]
        [MaxLength(CTConfig.User.MaxBioLength)]
        public string? Bio { get; set; }

        [PersonalData]
        public string? Avatar { get; set; }
        public string? AvatarId { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        
        [Required]
        public DateTime LastActive { get; set; } = DateTime.Now;

        public CommentsThread CommentsThread { get; set; } = new CommentsThread();
        public long CommentsThreadId { get; set; }

        public bool IsLoggedIn(ClaimsPrincipal claimsPrincipal)
        {
            return Id.ToString() == claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}    