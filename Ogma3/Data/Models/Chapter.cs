#nullable enable

using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Chapter
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int Order { get; set; }

        public DateTime PublishDate { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MinLength(1000)]
        [MaxLength(500_000)]
        public string Body { get; set; }

        [MaxLength(500)]
        public string? StartNotes { get; set; }

        [MaxLength(500)]
        public string? EndNotes { get; set; }
    }
}