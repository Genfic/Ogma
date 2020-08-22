#nullable enable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        
        [Required]
        public DateTime LastActive { get; set; } = DateTime.Now;

        public CommentsThread CommentsThread { get; set; } = new CommentsThread();

        public bool IsLoggedIn(ClaimsPrincipal claimsPrincipal)
        {
            return Id.ToString() == claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}    