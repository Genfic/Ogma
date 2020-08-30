using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class Document : BaseModel
    {
        [Required]
        public Guid GroupId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Slug { get; set; }
        
        [DefaultValue(null)]
        public DateTime? RevisionDate { get; set; }
        
        [Required]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        [Required]
        [DefaultValue(1)]
        public uint Version { get; set; }
        
        [Required]
        public string Body { get; set; }
    }
}