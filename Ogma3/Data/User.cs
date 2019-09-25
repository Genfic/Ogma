using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data
{
    public class User : IdentityUser
    {
        public string Title { get; set; }
        public string Bio { get; set; }
    }
}    