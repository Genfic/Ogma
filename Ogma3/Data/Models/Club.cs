using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Ogma3.Data.Models
{
    public class Club : BaseModel
    {
        [Required]
        [MinLength(CTConfig.CClub.MinNameLength)]
        [MaxLength(CTConfig.CClub.MaxNameLength)]
        public string Name { get; set; }
        
        [MaxLength(CTConfig.CClub.MaxHookLength)]
        public string Hook { get; set; }
        
        [MaxLength(CTConfig.CClub.MaxDescriptionLength)]
        public string Description { get; set; }
        
        public string Icon { get; set; }
        
        public string IconId { get; set; }
        
        [Required] 
        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        [JsonIgnore]
        public ICollection<ClubMember> ClubMembers { get; set; }
        
        [NotMapped]
        public IEnumerable<OgmaUser> Members
        {
            get => ClubMembers == null || ClubMembers.Count <= 0
                ? new List<OgmaUser>()
                : ClubMembers.Select(cm => cm.Member).ToList();
            set => ClubMembers = value.Select(u => new ClubMember
            {
                Member = u,
                MemberId = u.Id
            }).ToList();
        }

        public ICollection<ClubThread> Threads { get; set; }
        
    }
}