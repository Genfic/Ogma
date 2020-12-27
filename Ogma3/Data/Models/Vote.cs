namespace Ogma3.Data.Models
{
    public class Vote : BaseModel
    {
        public OgmaUser User { get; set; }
        public long UserId { get; set; }
        public long StoryId { get; set; }
    }
}