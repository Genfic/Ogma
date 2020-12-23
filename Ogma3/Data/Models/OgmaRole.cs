using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ogma3.Data.Models
{
    public class OgmaRole : IdentityRole<long>
    {
        [Required]
        [DefaultValue(false)]
        public bool IsStaff { get; set; }
        
        [DefaultValue(null)]
        public string? Color { get; set; }

        [DefaultValue(null)]
        public byte? Order { get; set; }

        public IEnumerable<OgmaUser> Users { get; set; }
    }
}