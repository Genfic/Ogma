using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Newtonsoft.Json;

namespace Ogma3.Data.Models
{
    public class Namespace
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(CTConfig.Namespace.MinNameLength)]
        [MaxLength(CTConfig.Namespace.MaxNameLength)]
        public string Name { get; set; }

        [MinLength(7)]
        [MaxLength(7)]
        public string Color { get; set; }
    }
}