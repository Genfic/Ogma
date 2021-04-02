using System;
using System.ComponentModel.DataAnnotations;
using Ogma3.Data.Bases;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Users;

namespace Ogma3.Data.Clubs
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