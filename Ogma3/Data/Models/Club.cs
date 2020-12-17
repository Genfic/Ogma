using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Ogma3.Data.Models
{
    public class Club : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CClub.MinNameLength)]
        [MaxLength(CTConfig.CClub.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }
        
        [MaxLength(CTConfig.CClub.MaxHookLength)]
        public string Hook { get; set; }
        
        [MaxLength(CTConfig.CClub.MaxDescriptionLength)]
        public string Description { get; set; }
        
        public string Icon { get; set; }
        
        public string IconId { get; set; }
        
        [Required] 
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [JsonIgnore] 
        public ICollection<ClubMember> ClubMembers { get; set; } = new List<ClubMember>();

        public ICollection<ClubThread> Threads { get; set; } = new List<ClubThread>();
        public ICollection<Folder> Folders { get; set; } = new List<Folder>();
    }
}