using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class OgmaRole : IdentityRole<long>
    {
        [DefaultValue(false)]
        public bool IsStaff { get; set; }
        
        [DefaultValue(null)]
        public string? Color { get; set; }

        [DefaultValue(null)]
        public byte? Order { get; set; }
    }
}