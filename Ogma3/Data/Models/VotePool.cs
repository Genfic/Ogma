using System.Collections.Generic;

namespace Ogma3.Data.Models
{
    public class VotePool : BaseModel
    {
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}