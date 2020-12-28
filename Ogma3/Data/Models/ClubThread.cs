
using System;
using System.ComponentModel.DataAnnotations;

namespace Ogma3.Data.Models
{
    public class ClubThread : BaseModel
    {
        [MinLength(CTConfig.CClubThread.MinTitleLength)]
        public string Title { get; set; }
        
        [MinLength(CTConfig.CClubThread.MinBodyLength)]
        public string Body { get; set; }
        
        public OgmaUser Author { get; set; }
        public long? AuthorId { get; set; }
        
        public DateTime CreationDate { get; set; }
        
        public CommentsThread CommentsThread { get; set; }

        public Club Club { get; set; }
        public long ClubId { get; set; }
    }
}