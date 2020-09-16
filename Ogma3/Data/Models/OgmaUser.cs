#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

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

        [NotMapped]
        public ICollection<OgmaRole> Roles
        {
            get => UserRoles == null || UserRoles.Count <= 0 
                ? new List<OgmaRole>() 
                : UserRoles.Select(ur => ur.Role).ToList(); 
            set => UserRoles = value.Select(r => new UserRole
            {
                Role = r,
                RoleId = r.Id
            }).ToList();
        }

        public bool IsLoggedIn(ClaimsPrincipal claimsPrincipal)
        {
            return Id.ToString() == claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}    