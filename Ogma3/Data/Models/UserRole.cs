using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class UserRole : IdentityUserRole<long>
    {
        public OgmaUser User { get; set; }
        public OgmaRole Role { get; set; }
    }
}