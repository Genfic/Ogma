using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class UserRole : IdentityUserRole<long>
    {
        [Required]
        public OgmaUser User { get; set; }
        [Required]
        public OgmaRole Role { get; set; }
    }
}