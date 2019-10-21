#nullable enable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class User : IdentityUser
    {
        [PersonalData]
        [MaxLength(CTConfig.User.MaxTitleLength)]
        public string? Title { get; set; } = null;
        
        [PersonalData]
        [MaxLength(CTConfig.User.MaxBioLength)]
        public string? Bio { get; set; } = null;

        [PersonalData]
        public string? Avatar { get; set; } = null;
        public string? AvatarId { get; set; } = null;
    }
}    