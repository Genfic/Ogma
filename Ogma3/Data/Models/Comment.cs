using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ogma3.Data.Models
{
    public class Comment
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] 
        public int CommentsThreadId { get; set; }

        [Required] 
        public User Author { get; set; }

        [Required] 
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Required]
        [MinLength(CTConfig.Comment.MinBodyLength)]
        [MaxLength(CTConfig.Comment.MaxBodyLength)]
        public string Body { get; set; }
    }
}