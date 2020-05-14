using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class InviteCode : BaseModel
    {
        private string _code;
        [Required]
        public string Code
        {
            get => _code;
            set
            {
                NormalizedCode = value.ToUpper();
                _code = value;
            }
        }

        [Required]
        public string NormalizedCode { get; set; }
        

        [DefaultValue(null)]
        public User? UsedBy { get; set; }
        public long? UsedById { get; set; }
        
        
        [DefaultValue(null)]
        public DateTime? UsedDate { get; set; }

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.Now;
    }
}