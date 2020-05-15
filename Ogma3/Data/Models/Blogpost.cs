using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Blogpost : BaseModel
    {
        [Required]
        [MinLength(CTConfig.Blogpost.MinTitleLength)]
        [MaxLength(CTConfig.Blogpost.MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        public string Slug { get; set; } = "";
        
        [Required]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required] 
        [DefaultValue(false)]
        public bool IsPublished { get; set; } = false;
        
        [Required]
        public User Author { get; set; }
        
        [Required]
        [MinLength(CTConfig.Blogpost.MinBodyLength)]
        [MaxLength(CTConfig.Blogpost.MaxBodyLength)]
        public string Body { get; set; }
        
        // Votes
        [Required]
        public  VotePool VotesPool { get; set; }
        
        public  CommentsThread CommentsThread { get; set; }
    }
}