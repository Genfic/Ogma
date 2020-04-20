#nullable enable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class InviteCode : BaseModel
    {
        [Required]
        public string Code { get; set; }

        [DefaultValue(null)]
        public User? UsedBy { get; set; }

        public string UsedById { get; set; }

        [Required] 
        [DefaultValue(null)]
        public DateTime? UsedDate { get; set; }

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.Now;
    }
}