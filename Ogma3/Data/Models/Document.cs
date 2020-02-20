using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class Document
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Slug { get; set; }
        
        [Required]
        public DateTime LastRevision { get; set; } = DateTime.Now;
        
        [Required]
        public DateTime CreationTime { get; set; } = DateTime.Now;
        
        [Required]
        public string Body { get; set; }
    }
}