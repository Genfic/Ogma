using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class User : IdentityUser
    {
        [MaxLength(20)]
        public string Title { get; set; }
        [MaxLength(5000)]
        public string Bio { get; set; }

        public string Avatar { get; set; }
        public string AvatarId { get; set; }
    }
}    