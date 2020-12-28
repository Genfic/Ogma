using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class Club : BaseModel, IReportableContent
    {
        [MinLength(CTConfig.CClub.MinNameLength)]
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Hook { get; set; }
        public string Description { get; set; }
        
        public string Icon { get; set; }
        public string IconId { get; set; }
        
        public DateTime CreationDate { get; set; }

        public ICollection<ClubMember> ClubMembers { get; set; }
        public ICollection<ClubThread> Threads { get; set; }
        public ICollection<Folder> Folders { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}