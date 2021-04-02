using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Ogma3.Data.Roles;

namespace Ogma3.Data.Users
{
    public class UserRole : IdentityUserRole<long>
    {
        [Required]
        public OgmaUser User { get; set; }
        [Required]
        public OgmaRole Role { get; set; }
    }
}